using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLibrary.Entities.DTOs;
using DatabaseLibrary.Entities.ProcurementProperties;

namespace Tests
{
    [TestClass]
    public class ProcurementsTests
    {

        void ProcurementsAreEqual(List<Procurement> lhs, List<Procurement> rhs)
        {
            Assert.AreEqual(lhs.Count, rhs.Count, $"quantities are not equal: {lhs.Count} != {rhs.Count}");
            for (int i = 0; i < lhs.Count; ++i) Assert.AreEqual(lhs[i].Id, rhs[i].Id, $"ids are not equal for index={i}");
        }

        [TestMethod]
        public async Task GetProcurementsManyByKind_PassAllKinds_ReturnsSameAsProcurementsBy()
        {
            // Arrange
            var kindsWithNames = new List<(KindOf kind, string name)>
            {
                (KindOf.ProcurementState, "Новый"),
                (KindOf.ShipmentPlane, "Следующая"),
                (KindOf.CorrectionDate, "Принят")
            };
            var kinds = new List<KindOf> { KindOf.Judgement, KindOf.FAS };

            // Act
            var expected = DatabaseLibrary.Queries.GET.View.ProcurementsBy("", KindOf.Applications);
            var real = await DatabaseLibrary.Controllers.GET.Procurements.Many.ByKind(KindOf.Applications);
            // Assert
            ProcurementsAreEqual (expected, real);

            foreach ((var kind, var kindName) in kindsWithNames)
            {
                // Act
                expected = DatabaseLibrary.Queries.GET.View.ProcurementsBy(kindName, kind);
                real = await DatabaseLibrary.Controllers.GET.Procurements.Many.ByKind(kind, kindName);
                // Assrt
                ProcurementsAreEqual(expected, real);

            }

            foreach (var kind in kinds)
            {
                // Act
                expected = DatabaseLibrary.Queries.GET.View.ProcurementsBy(kind);
                real = await DatabaseLibrary.Controllers.GET.Procurements.Many.ByKind(kind);
                // Assert
                ProcurementsAreEqual(expected, real);
            }
        }       
        
        [TestMethod]
        public async Task GetProcurementsManyByDateKind_PassAllKinds_ReturnsSameAsProcurementsBy()
        {            
            // Arrange
            var kindsWithNames = new List<(KindOf kind, string name)>
            {
                (KindOf.Deadline, "Отбой"),
                (KindOf.StartDate, "Проигран"),
                (KindOf.ResultDate, "Отклонен"),
                (KindOf.ContractConclusion, ""),
            };

            foreach ((var kind, var kindName) in kindsWithNames)
            {
                // Act
                bool isOverdue = true;
                var expected = DatabaseLibrary.Queries.GET.View.ProcurementsBy(kindName, isOverdue, kind);
                // Assert
                var real = await DatabaseLibrary.Controllers.GET.Procurements.Many.ByDateKind(kind, isOverdue, kindName);
            }
        }
    }
}
