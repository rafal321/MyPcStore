using System.Data.Entity;
using System.Linq;
using MyPcStore.Models.Data;

namespace MyPcStoreTests
{
    class TestPageDbset : TestDbSet<PageDTO>
    {
        public override PageDTO Find(params object[] keyValues)
        {
            return this.SingleOrDefault(x => x.Id == (int)keyValues.Single());
        }

    }
}