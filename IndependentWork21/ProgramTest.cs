using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IndependentWork21;

namespace IndependentWork21.Tests
{
    [TestClass]
    public class ProgramIntegrationTests
    {
        private sealed class RecordingObserver
        {
            public List<string> Received { get; } = new List<string>();
            public void OnDataProcessed(string data) => Received.Add(data);
        }

        [TestInitialize]
        public void Init()
        {
            ProcessingState.Instance.ResetForTests();
        }

        // Позитивний 1
        [TestMethod]
        public void Integration_AllPatterns_Upper_Works()
        {
            var publisher = new DataPublisher();
            var pipeline = new DataProcessingPipeline(publisher);

            var rec = new RecordingObserver();
            var len = new LengthLoggerObserver();

            publisher.DataProcessed += rec.OnDataProcessed;
            publisher.DataProcessed += len.OnDataProcessed;

            var result = pipeline.ProcessWithStrategy("upper", "Hello");

            Assert.AreEqual("HELLO", result);
            Assert.AreEqual(1, rec.Received.Count);
            Assert.AreEqual("HELLO", rec.Received[0]);
            Assert.AreEqual(1, len.InvocationCount);
            Assert.AreEqual(5, len.LastLength);
            Assert.AreEqual(1, ProcessingState.Instance.ProcessedCount);
            Assert.AreEqual("HELLO", ProcessingState.Instance.LastResult);
        }

        // Позитивний 2
        [TestMethod]
        public void Integration_RuntimeStrategySelection_Works()
        {
            var publisher = new DataPublisher();
            var pipeline = new DataProcessingPipeline(publisher);

            var r1 = pipeline.ProcessWithStrategy("upper", "AbC");
            var r2 = pipeline.ProcessWithStrategy("lower", "AbC");
            var r3 = pipeline.ProcessWithStrategy("reverse", "AbC");

            Assert.AreEqual("ABC", r1);
            Assert.AreEqual("abc", r2);
            Assert.AreEqual("CbA", r3);
            Assert.AreEqual(3, ProcessingState.Instance.ProcessedCount);
            Assert.AreEqual("CbA", ProcessingState.Instance.LastResult);
        }

        // Позитивний 3
        [TestMethod]
        public void Integration_SingletonState_StableInScenario()
        {
            var s1 = ProcessingState.Instance;
            var s2 = ProcessingState.Instance;

            Assert.AreSame(s1, s2);

            var publisher = new DataPublisher();
            var pipeline = new DataProcessingPipeline(publisher);

            pipeline.ProcessWithStrategy("lower", "ONE");
            pipeline.ProcessWithStrategy("lower", "TWO");

            Assert.AreEqual(2, s1.ProcessedCount);
            Assert.AreEqual("two", s1.LastResult);
        }

        // Негативний 1
        [TestMethod]
        public void Integration_UnknownStrategy_ThrowsArgumentException()
        {
            var publisher = new DataPublisher();
            var pipeline = new DataProcessingPipeline(publisher);

            Assert.ThrowsException<ArgumentException>(() =>
                pipeline.ProcessWithStrategy("unknown", "abc"));
        }

        // Негативний 2
        [TestMethod]
        public void Integration_NullInput_ThrowsArgumentNullException()
        {
            var publisher = new DataPublisher();
            var pipeline = new DataProcessingPipeline(publisher);

            Assert.ThrowsException<ArgumentNullException>(() =>
                pipeline.ProcessWithStrategy("upper", null!));
        }

        // Граничний
        [TestMethod]
        public void Integration_EmptyInput_Boundary_Works()
        {
            var publisher = new DataPublisher();
            var pipeline = new DataProcessingPipeline(publisher);

            var rec = new RecordingObserver();
            var len = new LengthLoggerObserver();

            publisher.DataProcessed += rec.OnDataProcessed;
            publisher.DataProcessed += len.OnDataProcessed;

            var result = pipeline.ProcessWithStrategy("reverse", string.Empty);

            Assert.AreEqual(string.Empty, result);
            Assert.AreEqual(1, rec.Received.Count);
            Assert.AreEqual(string.Empty, rec.Received[0]);
            Assert.AreEqual(0, len.LastLength);
            Assert.AreEqual(1, ProcessingState.Instance.ProcessedCount);
        }
    }
}