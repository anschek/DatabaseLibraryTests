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
    }
}