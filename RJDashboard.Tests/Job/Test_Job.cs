using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RJDashboard.Tests.Job
{
    public class Test_Job
    {
        [Fact]
        public void CreateNewJob_JobFile_IsNotNull()
        {
            RJController.Job.RJJob job = new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\4 head.job");

            var result = job.JobFile;

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateNewJob_With_Wrong_FileName_Throws_ArgumentException()
        {
            Exception ex = Assert.Throws<FileNotFoundException>(() => new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\xxx.job"));
            Assert.Equal("File not found", ex.Message);
        }

        [Fact]
        public void CreateNewJob_JobId_ReturnPropertyValue()
        {
            RJController.Job.RJJob job = new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\4 head.job");

            int result = job.JobId;

            Assert.Equal(1, result);
        }

        [Fact]
        public void CreateNewJob_With_Wrong_JobId_Throws_ArgumentException()
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new RJController.Job.RJJob(-1, "C:\\temp\\_ReaTest\\jobs\\4 head.job"));
            Assert.Equal("Parameter JobId must be > null", ex.Message);
        }

        [Fact]
        public void CreateNewJob_JobFile_ReturnPropertyValue()
        {
            RJController.Job.RJJob job = new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\4 head.job");

            var result = job.JobFile;

            Assert.Equal("4 head.job", result);
        }

        [Fact]
        public void CreateNewJob_Installation_ReturnPropertyValue()
        {
            RJController.Job.RJJob job = new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\4 head.job");

            var result = job.Installation;

            Assert.Equal("4 head.set", result);
        }

        [Fact]
        public void CreateNewJob_ReturnPropertyValueForGroupsCount()
        {
            RJController.Job.RJJob job = new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\bodende.job");

            var result = job.LabelManagement.Count;
            Assert.Equal(13, result);
        }

        [Fact]
        public void CreateNewJob_ReturnPropertyValueForObjectName()
        {
            RJController.Job.RJJob job = new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\bodende.job");

            var result = job.LabelManagement[1].ObjectName;
            Assert.Equal("Textbox_2", result);
        }

        [Fact]
        public void CreateNewJob_ReturnPropertyValueForContentName()
        {
            RJController.Job.RJJob job = new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\bodende.job");

            var result = job.LabelManagement[1].ContentName;
            Assert.Equal("Text _1", result);
        }

        [Fact]
        public void CreateNewJob_ReturnPropertyValueForContentValue()
        {
            RJController.Job.RJJob job = new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\bodende.job");

            var result = job.LabelManagement[1].ContentValue;
            Assert.Equal("BBBBBBBBBBBBBBBBBBBBBBBBBBBBB", result);
        }

        [Fact]
        public void CreateNewJob_ReturnPropertyValueForContentName_LastValue()
        {
            RJController.Job.RJJob job = new RJController.Job.RJJob(1, "C:\\temp\\_ReaTest\\jobs\\bodende.job");

            var result = job.LabelManagement[12].ContentValue;
            Assert.Equal("MMM", result);
        }
    }
}
