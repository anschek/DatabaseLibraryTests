using DatabaseLibrary.Entities.DTOs;

namespace Tests
{
    [TestClass]
    public class ProcurementsAggregateTests
    {
        [TestMethod]
        public async Task GetProcurementsCountByKind_PassAllKinds_ReturnsSameAsProcurementsCountBy()
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
            int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsCountBy("", KindOf.Applications);
            int realCount = await DatabaseLibrary.Controllers.GET.Procurements.Count.ByKind(KindOf.Applications);
            // Assert
            Assert.AreEqual(expectedCount, realCount);

            foreach ((var kind, var kindName) in kindsWithNames)
            {
                // Act
                expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsCountBy(kindName, kind);
                realCount = await DatabaseLibrary.Controllers.GET.Procurements.Count.ByKind(kind, kindName);
                // Assrt
                Assert.AreEqual(expectedCount, realCount);

            }

            foreach (var kind in kinds)
            {
                // Act
                expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsCountBy(kind);
                realCount = await DatabaseLibrary.Controllers.GET.Procurements.Count.ByKind(kind);
                // Assert
                Assert.AreEqual(expectedCount, realCount);
            }
        } 
        
        [TestMethod]
        public async Task GetProcurementsCountByStateAndStartDate_PassAllKinds_ReturnsSameAsProcurementsCountBy()
        {
            // Arrange
            var testStates = new string[] { "Выигран 1ч", "Отклонен" };
            var testDate = new DateTime(2024, 6, 15);

            foreach (var testState in testStates)
            {
                // Act
                int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsCountBy(testState, testDate);
                int realCount = await DatabaseLibrary.Controllers.GET.Procurements.Count.ByStateAndStartDate(testState, testDate);
                // Assert
                Assert.AreEqual(expectedCount, realCount);
            }
        }

        [TestMethod]
        public async Task GetProcurementsCountAccepted_PassAllKindsAndTerms_ReturnsSameAsProcurementsCountBy()
        {
            var testCases = new bool[] { true, false };
            foreach (var isOverdue in testCases)
            {            
                int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsCountBy(isOverdue);
                int realCount = await DatabaseLibrary.Controllers.GET.Procurements.Count.Accepted(isOverdue);
                Assert.AreEqual(expectedCount, realCount);
            }
        }


        [TestMethod]
        public async Task GetProcurementsCountCalculationQueue_ReturnsSameAsProcurementsQueueCount()
        {
            // Act
            int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsQueueCount();
            int realCount = await DatabaseLibrary.Controllers.GET.Procurements.Count.CalculationQueue();
            // Assert
            Assert.AreEqual(expectedCount, realCount);
        }

        [TestMethod]
        public async Task GetProcurementsCountManagersQueue_ReturnsSameAsProcurementsManagersQueueCount()
        {
            // Act
            int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsManagersQueueCount();
            int realCount = await DatabaseLibrary.Controllers.GET.Procurements.Count.ManagersQueue();
            // Assert
            Assert.AreEqual(expectedCount, realCount);

        }

        [TestMethod]
        public async Task GetProcurementsCountByVisa_PassAllKinds_ReturnsSameAsProcurementsCountBy()
        {
            // Arrange
            var testCases = new List<(KindOf, bool)> {
                            (KindOf.Calculating, true),
                            (KindOf.Calculating, false),
                            (KindOf.Purchase, true),
                            (KindOf.Purchase, false)
                        };

            foreach ((var kind, var stageCompleted) in testCases)
            {
                // Act
                int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsCountBy(stageCompleted, kind);
                int realCount = await DatabaseLibrary.Controllers.GET.Procurements.Count.ByVisa(kind, stageCompleted);
                // Assert
                Assert.AreEqual(expectedCount, realCount);
            }
        }

        [TestMethod]
        public async Task GetProcurementsCountByDateKind_PassAllKinds_ReturnsSameAsProcurementsountBy()
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
                int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsCountBy(kindName, isOverdue, kind);
                int realCount = await DatabaseLibrary.Controllers.GET.Procurements.Count.ByDateKind(kindName, isOverdue, kind);
                // Assert
                Assert.AreEqual(expectedCount, realCount);
            }
        }
    }
}
