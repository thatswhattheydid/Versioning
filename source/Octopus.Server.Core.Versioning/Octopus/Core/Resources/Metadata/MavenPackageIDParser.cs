﻿using System;
using Octopus.Core.Constants;
using Octopus.Core.Resources.Versioning;

namespace Octopus.Core.Resources.Metadata
{
    /// <summary>
    /// Maven package IDs come in the format: group#artifact
    /// </summary>
    public class MavenPackageIDParser : IPackageIDParser
    {
        public BasePackageMetadata GetMetadataFromPackageID(string packageID)
        {
            return BuildMetadata(packageID);
        }

        public bool CanGetMetadataFromPackageID(string packageID, out BasePackageMetadata metadata)
        {
            try
            {
                metadata = GetMetadataFromPackageID(packageID);
                return true;
            }
            catch
            {
                metadata = null;
                return false;
            }
        }

        public PackageMetadata GetMetadataFromPackageID(string packageID, string version, string extension)
        {
            var baseDetails = GetMetadataFromPackageID(packageID);
            return BuildMetadata(baseDetails.PackageId, version, extension);
        }

        public PhysicalPackageMetadata GetMetadataFromPackageID(
            string packageID,
            string version,
            string extension,
            long size,
            string hash)
        {
            return BuildMetadata(packageID, version, extension, size, hash);
        }

        public PackageMetadata GetMetadataFromPackageName(string packageFile, string[] extensions)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadata(packageFile, extensions),
                extensions);
        }

        public bool CanGetMetadataFromPackageName(string packageFile, string[] extensions,
            out PackageMetadata packageMetadata)
        {
            try
            {
                packageMetadata = GetMetadataFromPackageName(packageFile, extensions);
                return true;
            }
            catch
            {
                packageMetadata = null;
                return false;
            }
        }

        public Tuple<bool, PackageMetadata> CanGetMetadataFromPackageName(string packageFile, string[] extensions)
        {
            try
            {
                return new Tuple<bool, PackageMetadata>(true, GetMetadataFromPackageName(packageFile, extensions));
            }
            catch
            {
                return new Tuple<bool, PackageMetadata>(false, null);
            }
        }
        
        public Tuple<bool, PackageMetadata> CanGetMetadataFromServerPackageName(string packageFile, string[] extensions)
        {
            try
            {
                return new Tuple<bool, PackageMetadata>(true, GetMetadataFromServerPackageName(packageFile, extensions));
            }
            catch
            {
                return new Tuple<bool, PackageMetadata>(false, null);
            }
        }

        public PackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions)
        {
            return GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadataForServer(packageFile, extensions),
                extensions);
        }

        public bool CanGetMetadataFromServerPackageName(string packageFile, string[] extensions,
            out PackageMetadata packageMetadata)
        {
            try
            {
                packageMetadata = GetMetadataFromServerPackageName(packageFile, extensions);
                return true;
            }
            catch
            {
                packageMetadata = null;
                return false;
            }
        }

        public PhysicalPackageMetadata GetMetadataFromServerPackageName(string packageFile, string[] extensions,
            long size, string hash)
        {
            var baseDetails = GetMetadataFromPackageName(
                packageFile,
                PackageIdentifier.ExtractPackageExtensionAndMetadataForServer(packageFile, extensions),
                extensions);
            return BuildMetadata(baseDetails.PackageId, baseDetails.Version, baseDetails.FileExtension, size, hash);
        }

        PackageMetadata GetMetadataFromPackageName(string packageFile, Tuple<string, string> metadataAndExtension,
            string[] extensions)
        {
            var idAndVersion = metadataAndExtension.Item1;
            var extension = metadataAndExtension.Item2;

            if (string.IsNullOrEmpty(extension))
            {
                throw new Exception($"Unable to determine filetype of file \"{packageFile}\"");
            }

            var idAndVersionSplit = idAndVersion.Split(JavaConstants.MavenFilenameDelimiter);

            if (idAndVersionSplit.Length != 4 || idAndVersionSplit[0] != JavaConstants.MavenFeedPrefix)
            {
                throw new Exception(
                    $"Unable to extract the package ID and version from file \"{packageFile}\"");
            }

            return BuildMetadata(
                idAndVersionSplit[0] + JavaConstants.MavenFilenameDelimiter +
                idAndVersionSplit[1] + JavaConstants.MavenFilenameDelimiter +
                idAndVersionSplit[2],
                idAndVersionSplit[3],
                extension);
        }

        BasePackageMetadata BuildMetadata(string packageID)
        {
            var groupAndArtifact = packageID.Split(JavaConstants.MavenFilenameDelimiter);

            if (groupAndArtifact.Length != 3 || groupAndArtifact[0] != JavaConstants.MavenFeedPrefix)
            {
                throw new Exception(
                    $"Unable to extract the package ID and version from package ID \"{packageID}\"");
            }

            return new BasePackageMetadata()
            {
                PackageId = groupAndArtifact[0] + JavaConstants.MavenFilenameDelimiter +
                            groupAndArtifact[1] + JavaConstants.MavenFilenameDelimiter +
                            groupAndArtifact[2],
                FeedType = FeedType.Maven,
                PackageSearchPattern = packageID + "*"
            };
        }

        PackageMetadata BuildMetadata(string id, string version, string extension)
        {
            var baseMetadata = BuildMetadata(id);

            var pkg = new PackageMetadata();
            pkg.PackageId = baseMetadata.PackageId;
            pkg.Version = version;
            pkg.FileExtension = extension;
            pkg.FeedType = baseMetadata.FeedType;
            pkg.PackageSearchPattern = baseMetadata.PackageSearchPattern;
            pkg.PackageAndVersionSearchPattern = pkg.PackageId + JavaConstants.MavenFilenameDelimiter +
                                                 pkg.Version + "*";
            pkg.ServerPackageFileName = pkg.PackageId + JavaConstants.MavenFilenameDelimiter +
                                        pkg.Version + ServerConstants.SERVER_CACHE_DELIMITER;
            pkg.TargetPackageFileName = pkg.PackageId + JavaConstants.MavenFilenameDelimiter +
                                        pkg.Version + extension;
            pkg.VersionDelimiter = JavaConstants.MavenFilenameDelimiter.ToString();
            return pkg;
        }

        PhysicalPackageMetadata BuildMetadata(string id, string version, string extension, long size, string hash)
        {
            var basePackage = BuildMetadata(id, version, extension);
            var pkg = new PhysicalPackageMetadata();
            pkg.PackageId = basePackage.PackageId;
            pkg.Version = basePackage.Version;
            pkg.FileExtension = basePackage.FileExtension;
            pkg.FeedType = basePackage.FeedType;
            pkg.PackageAndVersionSearchPattern = basePackage.PackageAndVersionSearchPattern;
            pkg.PackageSearchPattern = basePackage.PackageSearchPattern;
            pkg.ServerPackageFileName = basePackage.ServerPackageFileName;
            pkg.TargetPackageFileName = basePackage.TargetPackageFileName;
            pkg.Size = size;
            pkg.Hash = hash;
            pkg.VersionDelimiter = basePackage.VersionDelimiter;
            return pkg;
        }
    }
}