using DatabaseLibrary.Entities.DTOs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ProcurementsEmployeesAggregateTests
    {
        [TestMethod]
        public async Task GetPeCountByStateAndStartDate_PassAllKindsAndEmployee_ReturnsSameAsPeCountBy()
        {
            // Arrange
            var testStates = new string[] { "Выигран 1ч", "Отклонен" };
            var testDate = new DateTime(2024, 6, 15);
            int employeeId = 36;

            foreach (var testState in testStates)
            {
                // Act
                int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsCountBy(testState, testDate, employeeId);
                int realCount = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Count.ByStateAndStartDate(testState, testDate, employeeId);
                // Assert
                Debug.WriteLine($"lhs count: {expectedCount} rhs count: {realCount}");
                Assert.AreEqual(expectedCount, realCount);
            }
        }

        [TestMethod]
        public async Task GetPeCountByKind_PassAllKindsAndEmployee_ReturnsSameAsPeCountBy()
        {
            // Arrange
            var kindsWithNames = new List<(KindOf kind, string name, int employee)>
            {
                (KindOf.ProcurementState, "Новый", 30),
                (KindOf.ShipmentPlane, "Следующая", 18),
                (KindOf.CorrectionDate, "Принят", 18)
            };
            var kinds = new List<KindOf> { KindOf.Judgement, KindOf.FAS };
            int employee = 19;

            // Act
            int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsEmployeesCountBy("", KindOf.Applications, employee);
            int realCount = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Count.ByKind(KindOf.Applications, employee);
            // Assert
            Debug.WriteLine($"lhs count: {expectedCount} rhs count: {realCount}");
            Assert.AreEqual(expectedCount, realCount);

            foreach ((var kind, var kindName, int employeeId) in kindsWithNames)
            {
                // Act
                expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsEmployeesCountBy(kindName, kind, employeeId);
                realCount = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Count.ByKind(kind, employeeId, kindName);
                // Assrt
                Debug.WriteLine($"lhs count: {expectedCount} rhs count: {realCount}");
                Assert.AreEqual(expectedCount, realCount);

            }

            foreach (var kind in kinds)
            {
                // Act
                expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsEmployeesCountBy(kind, employee);
                realCount = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Count.ByKind(kind, employee);
                // Assert
                Debug.WriteLine($"lhs count: {expectedCount} rhs count: {realCount}");
                Assert.AreEqual(expectedCount, realCount);
            }
        }

        [TestMethod]
        public async Task GetPeCountByDateKind_PassAllKindsAndTermAndEmployee_ReturnsSameAsPeCountBy()
        {
            // Arrange
            var kindsWithNames = new List<(KindOf kind, string name, int employee)>
            {
                (KindOf.Deadline, "Отбой", 30),
                (KindOf.StartDate, "Проигран", 32),
                (KindOf.ResultDate, "Отклонен", 19),
                (KindOf.ContractConclusion, "", 36),
            };

            foreach ((var kind, var kindName, int employee) in kindsWithNames)
            {
                // Act
                bool isOverdue = true;
                int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsEmployeesCountBy(kindName, isOverdue, kind, employee);
                int realCount = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Count.ByDateKind(kindName, isOverdue, kind, employee);
                // Assert
                Debug.WriteLine($"lhs count: {expectedCount} rhs count: {realCount}");
                Assert.AreEqual(expectedCount, realCount);
            }
        }        
        
        [TestMethod]
        public async Task GetPeCountAccepted_PassTermAndEmployee_ReturnsSameAsPeCountByAndNotPaid()
        {
            var testCases = new bool[] { true, false };
            int employee = 18;
            int expectedCount = 0;
            int realCount = 0;
            foreach (var isOverdue in testCases)
            {
                expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsEmployeesCountBy(isOverdue, employee);
                realCount = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Count.Accepted(employee, isOverdue);
                Debug.WriteLine($"lhs count: {expectedCount} rhs count: {realCount}");
                Assert.AreEqual(expectedCount, realCount);
            }

            expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsEmployeesCountNotPaid(employee);
            realCount = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Count.Accepted(employee);
            Debug.WriteLine($"lhs count: {expectedCount} rhs count: {realCount}");
            Assert.AreEqual(expectedCount, realCount);
        }        
        
        [TestMethod]
        public async Task GetPeCountByVisa_PassAllKindsAndEmployee_ReturnsSameAsPeCountBy()
        {
            // Arrange
            var testCases = new List<(KindOf, bool, int)> {
                (KindOf.Calculating, true, 32),
                (KindOf.Calculating, false, 36),
                (KindOf.Purchase, true, 36),
                (KindOf.Purchase, false, 19)
            };

            foreach ((var kind, var stageCompleted, int employee) in testCases)
            {
                // Act
                int expectedCount = DatabaseLibrary.Queries.GET.Aggregate.ProcurementsEmployeesCountBy(stageCompleted, kind, employee);
                int realCount = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Count.ByVisa(kind, stageCompleted, employee);
                // Assert
                Debug.WriteLine($"lhs count: {expectedCount} rhs count: {realCount}");
                Assert.AreEqual(expectedCount, realCount);
            }
        }

        void ProcurementsEmployeesGroupingsAreEqual(List<ProcurementsEmployeesGrouping> lhs, List<ProcurementsEmployeesGrouping> rhs)
        {
            Debug.WriteLine($"lhs count: {lhs.Count} rhs count: {rhs.Count}");
            Assert.AreEqual(lhs.Count, rhs.Count, $"quantities are not equal: {lhs.Count} != {rhs.Count}");
            for (int i = 0; i < lhs.Count; ++i)
            {
                Assert.AreEqual(lhs[i].Id, rhs[i].Id);
                decimal epsilon = 1;
                Assert.IsTrue(lhs[i].TotalAmount - epsilon <= rhs[i].TotalAmount && rhs[i].TotalAmount <= lhs[i].TotalAmount + epsilon);
            }
        }

        [TestMethod]
        public async Task GetPeGroupByPositions_PassPositions_ReturnsPeGroupBy()
        {
            // Arrange
            var positions = new string[] { "Специалист отдела расчетов", "Специалист по производству", "Специалист тендерного отдела" };
            // Act
            var expectedGroup = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesGroupBy(positions[0], positions[1], positions[2]);
            var realGroup = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Group.ByPositions(positions);
            // Assert
            ProcurementsEmployeesGroupingsAreEqual(expectedGroup, realGroup);
        }        
        
        [TestMethod]
        public async Task GetPeGroupByPositionsAndStates_PassPositionsAndStates_ReturnsPeGroupBy()
        {
            // Arrange
            var positions = new string[] { "Специалист отдела расчетов", "Специалист по производству", "Специалист тендерного отдела" };
            var states = new string[] { "Оформлен", "Приемка", "Выигран 1ч", "Посчитан" };
            // Act
            var expectedGroup = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesGroupBy(positions[0], positions[1], positions[2],
                states[0], states[1], states[2], states[3]);
            var realGroup = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Group.ByPositionsAndStates(positions, states);
            // Assert
            ProcurementsEmployeesGroupingsAreEqual(expectedGroup, realGroup);
        }
    }
}
