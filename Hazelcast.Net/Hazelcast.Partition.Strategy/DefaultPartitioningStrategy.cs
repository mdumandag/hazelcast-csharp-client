using Hazelcast.Core;
using Hazelcast.Partition.Strategy;


namespace Hazelcast.Partition.Strategy
{
	public class DefaultPartitioningStrategy : IPartitioningStrategy
	{
		public virtual object GetPartitionKey(object key)
		{
		    var aware = key as IPartitionAware<object>;
		    if (aware != null)
			{
                return aware.GetPartitionKey();
			}
			return null;
		}
	}
}