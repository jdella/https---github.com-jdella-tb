ureusing System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.Queries;

namespace TaskBuddi.Tests
{
    [TestFixture]
    public class Tests
    {
        AndroidApp app;
        int TestId = 0;
        AppResult[] result;

        [SetUp]
        public void BeforeEachTest()
        {
            app = ConfigureApp.Android.EnableLocalScreenshots().StartApp();	
            WaitForHome();
            TestId++;
        }
        //helper
        private void WaitForHome()
        {
            app.WaitForElement(c => c.Marked("action_bar_title").Text("TaskBuddi"));
        }

        [Test]
        [Category("UAT")]
        public void UatTests()
        {
            AddTaskGroup();
        
            TestId++;
            AddTask();
        }


       // [Test]
        [Category("TaskGroup")]
        public void AddTaskGroup()
        {
            var TEST_ID = 1;



            app.Tap(c => c.Marked("menu_add_taskGroup"));
            app.WaitForElement(c => c.Marked("Task Group Details"));

            app.EnterText(c => c.Marked("newGroupName"), "Test Group");
            app.Tap(c => c.Marked("menu_save"));
            WaitForHome();

            result = app.Query(c => c.Marked("Test Group"));
            Assert.IsTrue(result.Any());
            app.Screenshot("Test 1: Test Group entered");
        }

        //[Test]
        [Category("Task")]
        public void AddTask()
        {
            app.Repl();

            app.Tap(c => c.Marked("add new task"));
            app.WaitForElement(c => c.Marked("Task Details"));

            app.EnterText(c => c.Marked("vName"), "buy milk");
            app.ScrollDown("Library");
            app.ScrollDownTo(toMarked: "Library", withinMarked: "catSpinner");
            //TimeSpan.Ma
            app.EnterText(c => c.Marked("tdNotes"), new string('N', 3));
            app.Tap(c => c.Marked("menu_save"));
            WaitForHome();

            result = app.Query(c => c.Marked("TTT"));
            Assert.IsTrue(result.Any());
            app.Screenshot(TestId + "_TaskAdded");
            //app.WaitForElement(c => c.Marked("vName").Text("TTT"));
        }

        public void AddSecondGroup()
        {       
            // add 2nd group
            app.Tap(c => c.Marked("menu_add_taskGroup"));
            app.WaitForElement(c => c.Marked("Task Group Details"));
            app.EnterText(c => c.Marked("vName"), new string('2', 3));
            app.Tap(c => c.Marked("menu_save"));
            WaitForHome();

            result = app.Query(c => c.Marked("222"));
            Assert.IsTrue(result.Any());
            app.Screenshot(TestId + "_SecondGroupAdded");
        }

        public void NoteSaved()
        {   
            //task from 1st test
            app.Tap(c => c.Marked("TTT")); 
            app.WaitForElement(c => c.Marked("tdNotes")); 
            // check notes were saved
            result = app.Query(c => c.Marked("tdNotes").Text("NNN"));
            Assert.IsTrue(result.Any());    
            app.Screenshot(TestId + "_SavedTaskDetails");
        }

        public void EditAndMoveGroup()
        {
            // assign task to second group (should already be in Task Details Screen)
            app.Tap(c => c.Marked("tdAssignedTo"));
            app.Tap(c => c.Marked("vGroupName").Text("222"));
            // add "222" to Task Name
            app.EnterText(c => c.Marked("vName"), new string('2', 3));
            app.Tap(c => c.Marked("menu_save"));
            WaitForHome();
            // check if task in new group with new name
            result = app.Query(c => c.Marked("222").Sibling("RelativeLayout").Descendant("TextView").Text("TTT222"));
            Assert.IsTrue(result.Any());
            app.Screenshot(TestId + "_TaskNameAndGroupChange");
        }

        public void DeleteTask()
        {
            app.Tap(c => c.Marked("TTT222"));
            app.WaitForElement(c => c.Marked("Task Details"));
            app.Tap(c => c.Marked("menu_delete"));
            WaitForHome();

            result = app.Query(c => c.Marked("TTT222"));
            Assert.IsFalse(result.Any());
            app.Screenshot(TestId + "_TaskDeleted");
        }

        public void EditGroup()
        {
            //edit second group
            app.Tap(c => c.Marked("222"));
            app.WaitForElement(c => c.Marked("Task Group Details"));
            app.EnterText(c => c.Marked("vName"), new string('G', 3));
            app.Tap(c => c.Marked("menu_save"));
            WaitForHome();

            result = app.Query(c => c.Marked("222GGG"));
            Assert.IsTrue(result.Any());
            app.Screenshot(TestId + "_GroupEdited");
        }

        public void DeleteGroup()
        {
            // delete first group
            app.Tap(c => c.Marked("GGG"));
            app.WaitForElement(c => c.Marked("Task Group Details"));
            app.Tap(c => c.Marked("menu_delete"));
            WaitForHome();
            // confirm delete
            result = app.Query(c => c.Marked("GGG"));
            Assert.IsFalse(result.Any());
            app.Screenshot(TestId + "_GroupDeleted");       
        }

        // get location

        // plot marker'

        // go to location





    }
}

