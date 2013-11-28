using System;
using System.Threading;
using Hazelcast.Client.Test;
using Hazelcast.Core;
using Hazelcast.Net.Ext;
using Hazelcast.Transaction;
using NUnit.Framework;

namespace Hazelcast.Client.Test
{
	[TestFixture]
	public class ClientTxnTest:HazelcastBaseTest
	{

        [SetUp]
        public static void Init()
        {
            InitClient();
        }

        [TearDown]
        public static void Destroy()
        {
        }
		/// <exception cref="System.Exception"></exception>
		[Test,Ignore]
		public virtual void TestTxnRollback()
		{
			ITransactionContext context = client.NewTransactionContext();
            CountdownEvent latch = new CountdownEvent(1);
			try
			{
				context.BeginTransaction();
				Assert.IsNotNull(context.GetTxnId());
				var queue = context.GetQueue<object>("testTxnRollback");
				queue.Offer("item");
				//server.getLifecycleService().shutdown();
				context.CommitTransaction();
				Assert.Fail("commit should throw exception!!!");
			}
			catch (Exception)
			{
				context.RollbackTransaction();
				latch.Signal();
			}
			Assert.IsTrue(latch.Wait(TimeSpan.FromSeconds(10)));
			//
			IQueue<object> q = client.GetQueue<object>("testTxnRollback");
			Assert.IsNull(q.Poll());
			Assert.AreEqual(0, q.Count);
		}
	}
}