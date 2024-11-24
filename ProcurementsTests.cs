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
            Debug.WriteLine($"lhs count: {lhs.Count} rhs count: {rhs.Count}");
            Assert.AreEqual(lhs.Count, rhs.Count, $"quantities are not equal: {lhs.Count} != {rhs.Count}");
            for (int i = 0; i < lhs.Count; ++i) Assert.AreEqual(lhs[i].Id, rhs[i].Id);
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
                var real = await DatabaseLibrary.Controllers.GET.Procurements.Many.ByDateKind(kind, isOverdue, kindName);
                // Assert
                ProcurementsAreEqual(expected, real);
            }
        }        
        
        [TestMethod]
        public async Task GetProcurementsManyAccepted_PassAllKindsAndTerms_ReturnsSameAsProcurementsBy()
        {
            bool isOverdue = true;
            var expected = DatabaseLibrary.Queries.GET.View.ProcurementsBy(isOverdue);
            var real = await DatabaseLibrary.Controllers.GET.Procurements.Many.Accepted(isOverdue);
            ProcurementsAreEqual(expected, real);            
            
            isOverdue = false;
            expected = DatabaseLibrary.Queries.GET.View.ProcurementsBy(isOverdue);
            real = await DatabaseLibrary.Controllers.GET.Procurements.Many.Accepted(isOverdue);
            ProcurementsAreEqual(expected, real);

            expected = DatabaseLibrary.Queries.GET.View.ProcurementsNotPaid();
            real = await DatabaseLibrary.Controllers.GET.Procurements.Many.Accepted();
            ProcurementsAreEqual(expected, real);
        }        
        
        [TestMethod]
        public async Task GetProcurementsManyByVisa_PassAllKinds_ReturnsSameAsProcurementsBy()
        {
            // Arrange
            var testCases = new List<(KindOf, bool)> {
                (KindOf.Calculating, true),
                (KindOf.Calculating, false),
                (KindOf.Purchase, true),
                (KindOf.Purchase, false)
            };

            foreach((var kind, var stageCompleted) in testCases)
            {
                // Act
                var expected = DatabaseLibrary.Queries.GET.View.ProcurementsBy(stageCompleted, kind);
                var real = await DatabaseLibrary.Controllers.GET.Procurements.Many.ByVisa(kind, stageCompleted);
                // Assert
                ProcurementsAreEqual(expected, real);
            }
        }        
        
        [TestMethod]
        public async Task GetProcurementsManyCalculationQueue_ReturnsSameAsProcurementsQueue()
        {
            // Act
            var expected = DatabaseLibrary.Queries.GET.View.ProcurementsQueue();
            var real = await DatabaseLibrary.Controllers.GET.Procurements.Many.CalculationQueue();
            // Assert
            ProcurementsAreEqual(expected, real);
        }   
        
        [TestMethod]
        public async Task GetProcurementsManyManagersQueue_ReturnsSameAsProcurementsManagersQueue()
        {            
            // Act
            var expected = DatabaseLibrary.Queries.GET.View.ProcurementsManagersQueue();
            var real = await DatabaseLibrary.Controllers.GET.Procurements.Many.ManagersQueue();
            // Assert
            ProcurementsAreEqual(expected, real);

        }        
        [TestMethod]
        public async Task GetProcurementsManyByStateAndStartDate_PassSomeKinds_ReturnsSameAsProcurementsBy()
        {
            // Arrange
            var testStates = new string[] { "Выигран 1ч", "Отклонен" };
            var testDate = new DateTime(2024, 6, 15);

            foreach (var testState in testStates)
            {
                // Act
                var expected = DatabaseLibrary.Queries.GET.View.ProcurementsBy(testState, testDate);
                var real = await DatabaseLibrary.Controllers.GET.Procurements.Many.ByStateAndStartDate(testState, testDate);
                // Assert
                ProcurementsAreEqual(expected, real);
            }
        }

    }
}
