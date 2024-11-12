using DatabaseLibrary.Entities.DTOs;
using DatabaseLibrary.Entities.ProcurementProperties;
using System.Diagnostics;

namespace Tests
{
    [TestClass]
    public class HistoriesTests
    {
        [TestMethod]
        public async Task GetHistoriesGroupByProcurementState_PassStateWonPart2_ReturnsSameAsHistoryGroupBy()
        {
            // Arrange
            var histories = await DatabaseLibrary.Controllers.GET.Histories.Many();
            Assert.IsNotNull(histories);
            CollectionAssert.AllItemsAreNotNull(histories);
            string procurementState = "Выигран 2ч";

            // Act
            var firstGroups = DatabaseLibrary.Queries.GET.View.HistoryGroupBy(procurementState, histories).Item1;
            var secondGroups = await DatabaseLibrary.Controllers.GET.Histories.GroupByProcurementState(procurementState, histories);

            // Assert
            for (int i = 0; i < firstGroups.Count; ++i)
            {
                Assert.AreEqual(firstGroups[i].Item1, secondGroups[i].Year);
                Assert.AreEqual(firstGroups[i].Item2, secondGroups[i].Month);
                Assert.AreEqual(firstGroups[i].Item3, secondGroups[i].TotalAmount);
                Assert.AreEqual(firstGroups[i].Item4, secondGroups[i].TenderCount);

                for (int j = 0; j < secondGroups[i].TenderCount; ++j)
                    Assert.AreEqual(firstGroups[i].Item5[j].Id, secondGroups[i].Procurements[j].Id);
            }
        }

        [TestMethod]
        public async Task GetHistoriesGroupByProcurementState_PassStateCalculated_ReturnsSameAsHistoryGroupBy()
        {
            // Arrange
            var histories = await DatabaseLibrary.Controllers.GET.Histories.Many();
            string procurementState = "Посчитан";

            // Act
            var firstGroups = DatabaseLibrary.Queries.GET.View.HistoryGroupBy(procurementState, histories).Item1;
            var secondGroups = await DatabaseLibrary.Controllers.GET.Histories.GroupByProcurementState(procurementState, histories);

            // Assert
            for (int i = 0; i < firstGroups.Count; ++i)
            {
                Assert.AreEqual(firstGroups[i].Item1, secondGroups[i].Year);
                Assert.AreEqual(firstGroups[i].Item2, secondGroups[i].Month);
                Assert.AreEqual(firstGroups[i].Item3, secondGroups[i].TotalAmount);
                Assert.AreEqual(firstGroups[i].Item4, secondGroups[i].TenderCount);

                for (int j = 0; j < secondGroups[i].TenderCount; ++j)
                    Assert.AreEqual(firstGroups[i].Item5[j].Id, secondGroups[i].Procurements[j].Id);
            }
        }

        [TestMethod]
        public async Task GetHistoriesGroupByProcurementState_PassAllStates_Returns7CorrectStatesAndgroups()
        {
            // Arrange
            var histories = await DatabaseLibrary.Controllers.GET.Histories.Many();
            var targetStatuses = new List<string> { "Выигран 2ч", "Отправлен", "Оформлен", "Посчитан", "Новый", "Отбой", "Отклонен" };
            List<List<Tuple<int, int, decimal, int, List<Procurement>>>?> firstAllGroups = new();

            // Act
            foreach (string status in targetStatuses)
                firstAllGroups.Add(DatabaseLibrary.Queries.GET.View.HistoryGroupBy(status, histories).Item1);

            var secondAllGroups = await DatabaseLibrary.Controllers.GET.Histories.GroupByProcurementState(histories);

            // Assert
            Assert.AreEqual(targetStatuses.Count, secondAllGroups.Count);
            Assert.AreEqual(firstAllGroups.Count, secondAllGroups.Count);

            for (int groupsByState = 0; groupsByState < targetStatuses.Count; ++groupsByState)
            {
                var firstStateGroups = firstAllGroups[groupsByState];
                var secondStateGroups = secondAllGroups[targetStatuses[groupsByState]]; // with status from list statuses by index

                Assert.AreEqual(firstStateGroups.Count, secondStateGroups.Count);
                for (int innerGroup = 0; innerGroup < firstAllGroups[groupsByState].Count; ++innerGroup)
                {
                    var firstGroup = firstStateGroups[innerGroup];
                    var secondGroup = secondStateGroups[innerGroup];
                    Assert.AreEqual(firstGroup.Item1, secondGroup.Year);
                    Assert.AreEqual(firstGroup.Item2, secondGroup.Month);
                    Assert.AreEqual(firstGroup.Item3, secondGroup.TotalAmount);
                    Assert.AreEqual(firstGroup.Item4, secondGroup.TenderCount);

                    for (int procurementsInGroup = 0; procurementsInGroup < secondGroup.TenderCount; ++procurementsInGroup)
                        Assert.AreEqual(firstGroup.Item5[procurementsInGroup].Id, secondGroup.Procurements[procurementsInGroup].Id);
                }
                Debug.WriteLine($"group with state '{targetStatuses[groupsByState]}' complete successufully");
            }
        }
    }
}