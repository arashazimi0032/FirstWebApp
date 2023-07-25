﻿using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FirstWebApp.Helpers;
using FirstWebApp.Interfaces;
using Microsoft.Extensions.Options;

namespace FirstWebApp.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary? _cloudinary;
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            if (config.Value.CloudName.Length > 0 && config.Value.ApiKey.Length > 0 && config.Value.ApiSecret.Length > 0) 
            {
                var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
                );
                _cloudinary = new Cloudinary(acc);
            }
            else
            {
                _cloudinary = null;
            }
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file != null)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                    };
                    if (_cloudinary != null)
                    {
                        uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    }
                }
            }
            
            return uploadResult;
        }

        public async Task<DeletionResult> DetelePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result;
        }
    }
}
