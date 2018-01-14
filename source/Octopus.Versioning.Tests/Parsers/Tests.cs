﻿using System;
using NUnit.Framework;
using Octopus.Versioning;
using Octopus.Versioning.Metadata;

namespace Octopus.Server.Core.Versioning.Tests.Parsers
{
    [TestFixture]
    public class Tests
    {
        private readonly IPackageIDParser MavenParser = new MavenPackageIDParser();
        private readonly IPackageIDParser NuGetParser = new NuGetPackageIDParser();

        [Test]
        public void ParseNugetWithMaven()
        {
            Assert.Throws<Exception>(() =>
                MavenParser.GetMetadataFromPackageID("Acme.Web", "1.0.0", null, 0, "whatever"));
        }
        
        [Test]
        public void ParseMavenPackageId()
        {
            var metadata = MavenParser.GetMetadataFromPackageID("Maven#com.google.guava#guava");
            Assert.AreEqual(metadata.PackageId, "Maven#com.google.guava#guava");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Maven);
        }
        
        [Test]
        public void ParseNuGetPackageId()
        {
            var metadata = NuGetParser.GetMetadataFromPackageID("NuGet.Package");
            Assert.AreEqual(metadata.PackageId, "NuGet.Package");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Semver);
        }
        
        [Test]
        public void ParseMavenServerPackagePhysicalMetadata()
        {
            var filePath = "C:\\Maven#com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar";
            var metadata = MavenParser.GetMetadataFromServerPackageName(
                filePath, 
                new string[] {".jar"},
                123,
                "hash");
            Assert.AreEqual(metadata.FileExtension, ".jar");
            Assert.AreEqual(metadata.PackageId, "Maven#com.google.guava#guava");
            Assert.AreEqual(metadata.Version, "23.3-jre");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Maven);
            Assert.AreEqual(metadata.Size, 123);
            Assert.AreEqual(metadata.Hash, "hash");
        }        
        
        /// <summary>
        /// The path used here incorrectly has a perido between artifact and version, and
        /// this should not parse correctly.
        /// </summary>
        [Test]
        public void FailParseMavenServerPackagePhysicalMetadata()
        {
            var filePath = "C:\\Temp\\Files\\feeds-maven\\Maven#com.google.guava#guava.22.0_3C7672DD8977C04DBD1F8BA70E1AF190.jar";
            Assert.Throws<Exception>(() => MavenParser.GetMetadataFromServerPackageName(
                filePath,
                new string[] {".jar"},
                123,
                "hash"));
        }
        
        [Test]
        public void ParseNuGetServerPackagePhysicalMetadata()
        {
            var filePath = "C:\\package.suffix.1.0.0_9822965F2883AD43AD79DA4E8795319F.zip";
            var metadata = NuGetParser.GetMetadataFromServerPackageName(
                filePath, 
                new string[] {".zip"},
                123,
                "hash");
            Assert.AreEqual(metadata.FileExtension, ".zip");
            Assert.AreEqual(metadata.PackageId, "package.suffix");
            Assert.AreEqual(metadata.Version, "1.0.0");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Semver);
            Assert.AreEqual(metadata.Size, 123);
            Assert.AreEqual(metadata.Hash, "hash");
        }
        
        [Test]
        public void ParseMavenServerPackage()
        {
            var filePath = "C:\\Maven#com.google.guava#guava#23.3-jre_9822965F2883AD43AD79DA4E8795319F.jar";
            var metadata = MavenParser.GetMetadataFromServerPackageName(filePath, new string[] {".jar"});
            Assert.AreEqual(metadata.FileExtension, ".jar");
            Assert.AreEqual(metadata.PackageId, "Maven#com.google.guava#guava");
            Assert.AreEqual(metadata.Version, "23.3-jre");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Maven);
        }
        
        [Test]
        public void ParseMavenTargetPackage()
        {
            var filePath = "C:\\Maven#com.google.guava#guava#22.0.jar-e55fcd51-6081-4300-91a3-117b7930c023";
            var metadata = MavenParser.GetMetadataFromPackageName(filePath, new string[] {".jar"});
            Assert.AreEqual(metadata.FileExtension, ".jar");
            Assert.AreEqual(metadata.PackageId, "Maven#com.google.guava#guava");
            Assert.AreEqual(metadata.Version, "22.0");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Maven);
        }
        
        [Test]
        public void MavenFailToParseNuGetTargetPackage()
        {
            Assert.Throws<Exception>(() =>
            {
                var filePath = "C:\\package.suffix.1.0.0.zip-e55fcd51-6081-4300-91a3-117b7930c023";
                var metadata = MavenParser.GetMetadataFromPackageName(filePath, new string[] {".jar"});
            });
        }
        
        [Test]
        public void ParseNugetServerPackage()
        {
            var filePath = "C:\\package.suffix.1.0.0_9822965F2883AD43AD79DA4E8795319F.zip";
            var metadata = NuGetParser.GetMetadataFromServerPackageName(filePath, new string[] {".zip"});
            Assert.AreEqual(metadata.FileExtension, ".zip");
            Assert.AreEqual(metadata.PackageId, "package.suffix");
            Assert.AreEqual(metadata.Version, "1.0.0");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Semver);
        }
        
        [Test]
        public void ParseNugetTargetPackage()
        {
            var filePath = "C:\\package.suffix.1.0.0.zip-e55fcd51-6081-4300-91a3-117b7930c023";
            var metadata = NuGetParser.GetMetadataFromPackageName(filePath, new string[] {".zip"});
            Assert.AreEqual(metadata.FileExtension, ".zip");
            Assert.AreEqual(metadata.PackageId, "package.suffix");
            Assert.AreEqual(metadata.Version, "1.0.0");
            Assert.AreEqual(metadata.VersionFormat, VersionFormat.Semver);
        }
        
        [Test]
        public void NuGetFailToParseMavenTargetPackage()
        {
            Assert.Throws<Exception>(() =>
            {
                var filePath = "C:\\Maven#com.google.guava#guava#22.0.jar-e55fcd51-6081-4300-91a3-117b7930c023";
                var metadata = NuGetParser.GetMetadataFromPackageName(filePath, new string[] {".jar"});
            });           
        }
    }
}