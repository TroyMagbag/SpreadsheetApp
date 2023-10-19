// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using SpreadsheetEngine;

namespace LibraryTests.Tests1
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestGetCell()
        {
            Spreadsheets newSheet = new Spreadsheets(30, 30);
            SpreadsheetCell temp = (SpreadsheetCell) newSheet.GetCell(10,20);
            Assert.AreEqual(temp.RowIndex, 10); //should pass 
            Assert.IsNull(newSheet.GetCell(100, 100)); //should pass
        }
        [Test]
        public void TestTree()
        {
            ExpressionTree testTree = new ExpressionTree("5+5"); //sucess
            Assert.AreEqual(10, testTree.Evaluate());
            ExpressionTree testTree2 = new ExpressionTree("(2+5)*2"); //sucess
            Assert.AreEqual(14, testTree2.Evaluate());
            ExpressionTree testTree3 = new ExpressionTree("()"); //sucess
            Assert.Throws<System.InvalidOperationException>(() => testTree3.Evaluate()); ///sucess 
        }

    }
}
