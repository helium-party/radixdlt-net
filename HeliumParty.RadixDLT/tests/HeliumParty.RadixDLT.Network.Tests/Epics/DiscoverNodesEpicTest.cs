using HeliumParty.RadixDLT.Epics;
using HeliumParty.RadixDLT.Universe;
using Shouldly;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Xunit;
using System.Linq;
using akarnokd.reactive_extensions;

namespace HeliumParty.RadixDLT.Network.Tests.Epics
{
    public class DiscoverNodesEpicTest
    {
        readonly RadixUniverseConfig testUniverseConfig;
        readonly RadixNode testNode;

        public DiscoverNodesEpicTest()
        {
            testUniverseConfig = new RadixUniverseConfig("", "", null, 0, RadixUniverseType.Development, 0, 0, null);
            testNode = new RadixNode("", 1);
        }

        [Fact]
        public void WhenSeedReturnAnError_EpicShouldNotFail()
        {
            // prepare
            var seeds = Observable.Throw<RadixNode>(new Exception("Bad exception!"));
            var epic = new DiscoverNodesEpic(seeds, testUniverseConfig);

            var actions = new ReplaySubject<IRadixNodeAction>();
            var networkState = Observable.Return(
                new RadixNetworkState(
                    new System.Collections.Generic.Dictionary<RadixNode, RadixNodeState>()));
            var output = epic.Epic(actions, networkState);

            var testObserver = new TestObserver<IRadixNodeAction>();

            // act
            output.Subscribe(testObserver);
            actions.OnNext(new Actions.DiscoverMoreNodesAction());

            // assert
            testObserver.AssertNoError();
            testObserver.Items.First().ShouldBeOfType<Actions.DiscoverMoreNodesErrorAction>();
        }

        [Fact]
        public void WhenSeedsReturnANonMatchingUniverse_ANodeMismatchUniverseEventShouldBeEmitted()
        {
            // prepare
            var seeds = Observable.Return(testNode);
            var badUniverseConfig = new RadixUniverseConfig("", "", null, 0, RadixUniverseType.Development, 0, 0, null);

            var actions = new ReplaySubject<IRadixNodeAction>();
            var networkState = Observable.Return(
                new RadixNetworkState(
                    new System.Collections.Generic.Dictionary<RadixNode, RadixNodeState>()));

            var epic = new DiscoverNodesEpic(seeds, testUniverseConfig);
            var output = epic.Epic(actions, networkState);

            var testObserver = new TestObserver<IRadixNodeAction>();

            // act
            output.Subscribe(testObserver);
            actions.OnNext(new Actions.DiscoverMoreNodesAction());

            var firstAction = testObserver.Items[0];
            actions.OnNext(new Actions.GetUniverseResponseAction(testNode, badUniverseConfig));
            var secondAction = testObserver.Items[1];

            // assert
            firstAction.ShouldBeOfType<Actions.GetUniverseRequestAction>();
            secondAction.ShouldBeOfType<Actions.NodeUniverseMismatchAction>();
        }
    }
}
