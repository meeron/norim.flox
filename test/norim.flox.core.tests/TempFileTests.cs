using System;
using System.IO;
using Xunit;

namespace norim.flox.core.tests
{
    public class UnitTest1
    {
        [Fact]
        public void Create_Succes()
        {
            var tempFilePath = string.Empty;

            using(var tempFile = TempFile.Create())
            {
                tempFilePath = tempFile.Path;

                File.WriteAllText(tempFile.Path, "Tests");
                Assert.True(File.Exists(tempFile.Path));
            }

            Assert.False(File.Exists(tempFilePath));
        }
    }
}
