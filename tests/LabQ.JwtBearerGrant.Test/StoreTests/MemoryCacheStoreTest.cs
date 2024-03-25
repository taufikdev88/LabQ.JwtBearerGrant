using LabQ.JwtBearerGrant.Test.Warmups;

namespace LabQ.JwtBearerGrant.Test.StoreTests;
public class MemoryCacheStoreTest(WarmupMemoryCacheStore warmup) : GenericStoreServiceTest<WarmupMemoryCacheStore>(warmup)
{
}
