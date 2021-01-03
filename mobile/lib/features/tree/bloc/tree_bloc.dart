import 'dart:async';

import 'package:bloc/bloc.dart';
import 'package:flutter/foundation.dart';
import 'package:graphite/graphite.dart';
import 'package:tree_of_a_kind/contracts/people/contracts.dart';
import 'package:tree_of_a_kind/contracts/people/people_repository.dart';
import 'package:tree_of_a_kind/contracts/tree/contracts.dart';
import 'package:tree_of_a_kind/contracts/tree/tree_repository.dart';

part 'tree_event.dart';
part 'tree_state.dart';

class TreeBloc extends Bloc<TreeEvent, TreeState> {
  TreeBloc({@required this.treeRepository, @required this.peopleRepository})
      : super(LoadingState());

  final TreeRepository treeRepository;
  final PeopleRepository peopleRepository;

  TreeDTO _tree;
  List<NodeInput> _treeGraph;

  @override
  Stream<TreeState> mapEventToState(
    TreeEvent event,
  ) async* {
    if (event is FetchTree) {
      yield* _handleFetchTree(event.treeId);
    } else if (event is PersonRemoved) {
      yield* _handlePersonRemoved(event.personId);
    } else {
      throw new Exception("Unhandled event.");
    }
  }

  List<NodeInput> _treeToGraph(TreeDTO tree) {
    return tree.people
        .map((person) => NodeInput(
            id: person.id,
            next: [
              // if (person.spouse != null) person.spouse,
              ...person.children,
            ].where((element) => element != null).toList()))
        .toList();
  }

  Stream<TreeState> _handleFetchTree(String treeId) async* {
    yield const LoadingState();

    final result = await treeRepository.getTreeDetails(treeId: treeId);

    if (result.unexpectedError) {
      yield const UnknownErrorState();
    } else {
      _tree = result.data;
      _tree.people
          .sort((p1, p2) => (p1.spouse ?? '').compareTo(p2.spouse ?? ''));
      _treeGraph = _treeToGraph(_tree);
      yield PresentingTree(_tree, _treeGraph);
    }
  }

  Stream<TreeState> _handlePersonRemoved(String personId) async* {
    yield const LoadingState();

    final result = await peopleRepository.removePerson(
        treeId: _tree.treeId, personId: personId);

    if (result.unexpectedError) {
      yield const UnknownErrorState();
    } else {
      yield* _handleFetchTree(_tree.treeId);
    }
  }
}
