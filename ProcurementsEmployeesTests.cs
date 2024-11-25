using DatabaseLibrary.Entities.DTOs;
using DatabaseLibrary.Entities.EmployeeMuchToMany;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ProcurementsEmployeesTests
    {
        void ProcurementsEmployeesAreEqual(List<ProcurementsEmployee> lhs, List<ProcurementsEmployee> rhs)
        {
            Debug.WriteLine($"lhs count: {lhs.Count} rhs count: {rhs.Count}");
            Assert.AreEqual(lhs.Count, rhs.Count, $"quantities are not equal: {lhs.Count} != {rhs.Count}");
            for (int i = 0; i < lhs.Count; ++i)
            {
                Assert.AreEqual(lhs[i].Id, rhs[i].Id, $"pe[{i}] ids are not equal: {lhs[i].Id} != {rhs[i].Id}");
                Assert.AreEqual(lhs[i].ProcurementId, rhs[i].ProcurementId);
                Assert.AreEqual(lhs[i].EmployeeId, rhs[i].EmployeeId);
            }
        }

        [TestMethod]
        public async Task GetPeManyByKind_PassAllKindsAndEmployee_ReturnsSameAsPeBy()
        {
            // Arrange
            var kindsWithNames = new List<(KindOf kind, string name, int employee)>
            {
                 (KindOf.ProcurementState, "Новый", 30),
                 (KindOf.ShipmentPlane, "Следующая", 19),
                 (KindOf.CorrectionDate, "Принят", 18)
            };
            var kinds = new List<KindOf> { KindOf.Judgement, KindOf.FAS };
            int employee = 19;
            // Act
            var expected = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesBy("", KindOf.Applications, employee);
            var real = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Many.ByKind(KindOf.Applications, employee);
            // Assert
            ProcurementsEmployeesAreEqual(expected, real);

            foreach ((var kind, var kindName, var employeeId) in kindsWithNames)
            {
                // Act
                expected = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesBy(kindName, kind, employeeId);
                real = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Many.ByKind(kind, employeeId, kindName);
                // Assrt
                ProcurementsEmployeesAreEqual(expected, real);

            }

            foreach (var kind in kinds)
            {
                // Act
                expected = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesBy(kind, 33);
                real = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Many.ByKind(kind, 33);
                // Assert
                ProcurementsEmployeesAreEqual(expected, real);
            }
        }        
        [TestMethod]
        public async Task GetPeManyByDateKind_PassAllKindsAndEmployee_ReturnsSameAsPeBy()
        {
            // Arrange
            var kindsWithNames = new List<(KindOf kind, string name, int employee)>
            {
                (KindOf.Deadline, "Отбой", 30),
                (KindOf.StartDate, "Проигран", 32),
                (KindOf.ContractConclusion, "", 19),
            };

            foreach ((var kind, var kindName, int employeeId) in kindsWithNames)
            {
                // Act
                bool isOverdue = true;
                var expected = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesBy(kindName, isOverdue, kind, employeeId);
                var real = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Many.ByDateKind(kindName, isOverdue, kind, employeeId);
                // Assert
                ProcurementsEmployeesAreEqual(expected, real);
            }
        }        
        [TestMethod]
        public async Task GetPeManyAccepted_PassTermAndEmployee_ReturnsSameAsPeByAndPeNotPaid()
        {
            bool isOverdue = true;
            int employee = 19;
            var expected = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesBy(isOverdue, employee);
            var real = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Many.Accepted(employee, isOverdue);
            ProcurementsEmployeesAreEqual(expected, real);

            isOverdue = false;
            employee = 32;
            expected = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesBy(isOverdue, employee);
            real = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Many.Accepted(employee, isOverdue);
            ProcurementsEmployeesAreEqual(expected, real);

            employee = 19;
            expected = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesNotPaid(employee);
            real = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Many.Accepted(employee);
            ProcurementsEmployeesAreEqual(expected, real);
        }        
        
        [TestMethod]
        public async Task GetPeManyByVisa_PassAllKindsAndTermAndEmployee_ReturnsSameAsPeBy()
        {
            // Arrange
            var testCases = new List<(KindOf, bool, int)> 
            {
                (KindOf.Calculating, true, 32),
                (KindOf.Calculating, false, 36),
                (KindOf.Purchase, true, 36),
                (KindOf.Purchase, false, 19)
            };

            foreach ((var kind, var stageCompleted, int employeeId) in testCases)
            {
                // Act
                var expected = DatabaseLibrary.Queries.GET.View.ProcurementsEmployeesBy(stageCompleted, kind, employeeId);
                var real = await DatabaseLibrary.Controllers.GET.ProcurementsEmployees.Many.ByVisa(kind, stageCompleted, employeeId);
                // Assert
                ProcurementsEmployeesAreEqual(expected, real);
            }
        }
    }
}
