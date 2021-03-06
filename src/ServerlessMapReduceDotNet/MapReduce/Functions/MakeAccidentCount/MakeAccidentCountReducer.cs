using System.Linq;
using ServerlessMapReduceDotNet.MapReduce.Abstractions;
using ServerlessMapReduceDotNet.Model;

namespace ServerlessMapReduceDotNet.MapReduce.Functions.MakeAccidentCount
{
    class MakeAccidentCountReducer : IReducerFunc
    {
        public KeyValuePairCollection Reduce(KeyValuePairCollection inputKeyValuePairs)
        {
            var countKvps = inputKeyValuePairs
                .GroupBy(x => ((CountKvp) x).Key)
                .Select(x => (IKeyValuePair)new CountKvp(
                        x.Key,
                        x.Sum(y => ((CountKvp) y).Value)
                    )
                );

            return new KeyValuePairCollection(countKvps);
        }
    }
}