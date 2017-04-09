using System;
using System.Collections.Generic;
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
            RJController.Job.Job job = new RJController.Job.Job(1, "C:\\temp\\_ReaTest\\jobs\\4 head.job");

            var result = job.JobFile;

            Assert.NotNull(result);
        }

        [Fact]
        public void CreateNewJob_JobId_ReturnPropertyValue()
        {
            RJController.Job.Job job = new RJController.Job.Job(1, "C:\\temp\\_ReaTest\\jobs\\4 head.job");

            int result = job.JobId;

            Assert.Equal(1, result);
        }

        [Fact]
        public void CreateNewJob_With_Wrong_JobId_Throws_ArgumentException()
        {
            Exception ex = Assert.Throws<ArgumentException>(() => new RJController.Job.Job(-1, "C:\\temp\\_ReaTest\\jobs\\4 head.job"));
            Assert.Equal("Parameter JobId must be > null", ex.Message);
        }

        [Fact]
        public void CreateNewJob_JobFile_ReturnPropertyValue()
        {
            RJController.Job.Job job = new RJController.Job.Job(1, "C:\\temp\\_ReaTest\\jobs\\4 head.job");

            var result = job.JobFile;

            Assert.Equal("4 head.job", result);
        }

        [Fact]
        public void CreateNewJob_Installation_ReturnPropertyValue()
        {
            RJController.Job.Job job = new RJController.Job.Job(1, "C:\\temp\\_ReaTest\\jobs\\4 head.job");

            var result = job.Installation;

            Assert.Equal("4 head.set", result);
        }

        [Fact]
        public void CreateNewJob_Group_ReturnPropertyCount()
        {
            RJController.Job.Job job = new RJController.Job.Job(1, "C:\\temp\\_ReaTest\\jobs\\4 head.job");

            var result = job.Groups.Count;

            Assert.Equal(4, result);
        }

        [Fact]
        public void CreateNewJob_Group_ReturnPropertyValue()
        {
            RJController.Job.Job job = new RJController.Job.Job(1, "C:\\temp\\_ReaTest\\jobs\\demojob_4ph.job");

            var result = job.Groups["Front"].ToString();
            Assert.Equal(@"C:\\labels\demolabel_4ph.xml", result);
        }
    }
}
