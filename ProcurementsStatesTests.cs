using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ProcurementsStatesTests
    {
        [TestMethod]
        public async Task GetProcurementsStatesManyByEmployeePosition_PassAllEmployeePositions_ReturnsSameAsDistributionOfProcurementStates()
        {
            // Arrange
            var employeePositions = new string[]{
                 "Администратор","Руководитель отдела расчетов","Заместитель руководителя отдела расчетов", "Специалист отдела расчетов",
                    "Руководитель тендерного отдела","Заместитель руководителя тендреного отдела","Специалист тендерного отдела",
                    "Специалист по работе с электронными площадками","Руководитель отдела закупки","Заместитель руководителя отдела закупок",
                    "Специалист закупки", "Руководитель отдела производства","Заместитель руководителя отдела производства","Специалист по производству",
                    "Юрист"
            };

            foreach (var position in employeePositions)
            {
                // Act
                var expected = DatabaseLibrary.Queries.GET.View.DistributionOfProcurementStates(position);
                var real = await DatabaseLibrary.Controllers.GET.ProcurementStates.ManyByEmployeePosition(position);
                // Assert
                Assert.AreEqual(expected.Count, real.Count);
                for(int i=0; i<expected.Count; i++) Assert.AreEqual(expected[i].Id, real[i].Id);
            }
        }
    }
}
