﻿using System;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;

namespace VisionSample
{
    public class VisionSampleBase<TImageProcessor> : IVisionSample where TImageProcessor : new()
    {
        byte[] _model;
        string _name;
        string _modelName;
        Task _initializeTask;
        TImageProcessor _imageProcessor;
        InferenceSession _session;
        ExecutionProviderOptions _curExecutionProvider;

        public VisionSampleBase(string name, string modelName)
        {
            _name = name;
            _modelName = modelName;
            _ = InitializeAsync();
        }

        public string Name => _name;
        public string ModelName => _modelName;
        public byte[] Model => _model;
        public InferenceSession Session => _session;
        public TImageProcessor ImageProcessor => _imageProcessor ??= new TImageProcessor();

	    public Task UpdateExecutionProvider(ExecutionProviderOptions executionProvider)
	    {
            // creating the inference session can be expensive and should be done as a one-off.
            // additionally each session uses memory for the model and the infrastructure required to execute it,
            // and has its own threadpools.
	        return Task.Run(() =>
	                        {
                                if (executionProvider == _curExecutionProvider)
                                    return;

	                            if (executionProvider == ExecutionProviderOptions.CPU)
	                            {
                                    // create session that uses the CPU execution provider
	                                _session = new InferenceSession(_model);
	                            }
	                            else
	                            {
                                    // create session that uses the NNAPI/CoreML. the CPU execution provider is also
                                    // enabled by default to handle any parts of the model taht NNAPI/CoreML cannot.
	                                var options = SessionOptionsContainer.Create(nameof(ExecutionProviderOptions.Platform));
	                                _session = new InferenceSession(_model, options);
	                            }
	                        });
	    }

        protected virtual Task<ImageProcessingResult> OnProcessImageAsync(byte[] image) => throw new NotImplementedException();

        public Task InitializeAsync()
        {
            if (_initializeTask == null || _initializeTask.IsFaulted)
                _initializeTask = Task.Run(() => Initialize());

            return _initializeTask;
        }

        public async Task<ImageProcessingResult> ProcessImageAsync(byte[] image)
        {
            await InitializeAsync().ConfigureAwait(false);
            return await OnProcessImageAsync(image);
        }

        void Initialize()
        {
            _model = ResourceLoader.GetEmbeddedResource(ModelName);
            _session = new InferenceSession(_model);  // default to CPU 
            _curExecutionProvider = ExecutionProviderOptions.CPU;
        }
    }
}
