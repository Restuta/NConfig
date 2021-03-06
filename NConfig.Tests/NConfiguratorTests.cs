﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NConfig;
using System.Configuration;

namespace NConfig.Tests
{
    [TestFixture]
    public class NConfiguratorTests
    {

        [Test]
        public void Should_provide_default_ConectStrings_for_non_existing_file()
        {
            var connStrings = NConfigurator.UsingFile("NotExisting.config").ConnectionStrings;

            Assert.That(connStrings, Is.EqualTo(ConfigurationManager.ConnectionStrings));
        }

        [Test]
        public void Should_provide_default_AppSettings_for_non_existing_file()
        {
            var settings = NConfigurator.UsingFile("NotExisting.config").AppSettings; 

            Assert.That(settings, Is.EqualTo(ConfigurationManager.AppSettings));
        }

        [Test]
        public void Should_merge_default_AppSettings_with_defined_in_file()
        {
            var settings = NConfigurator.UsingFile("Configs//NConfigTest.config").AppSettings;

            Assert.That(settings.Count, Is.EqualTo(3));
            Assert.That(settings["Test"], Is.EqualTo("NConfigTest.Value"));
        }

        [Test]
        public void Should_merge_default_ConectStrings_with_defined_in_file()
        {
            var connStrings = NConfigurator.UsingFile("Configs//NConfigTest.config").ConnectionStrings;

            Assert.That(connStrings.Count, Is.EqualTo(4));
            Assert.That(connStrings["TestConnectString"].ConnectionString, Is.EqualTo("NConfigTest.ConnectString"));
        }

        [Test]
        public void Should_override_sections_by_the_most_recent_config_file()
        {
            var testSection = NConfigurator.UsingFiles("Configs//Aliased.config", "Configs//NConfigTest.config").GetSection<TestSection>();

            Assert.That(testSection.Value, Is.EqualTo("Tests.Aliased.Value"));
        }

        [Test]
        public void Should_override_ConnectString_by_the_most_recent_config_file()
        {
            var conString = NConfigurator.UsingFiles("Configs//Aliased.config", "Configs//NConfigTest.config").ConnectionStrings["TestConnectString"].ConnectionString;

            Assert.That(conString, Is.EqualTo("Aliased.ConnectString"));
        }

        [Test]
        public void Should_corresponds_to_ConfigurationManager_by_default()
        {
            Assert.That(NConfigurator.Default.ConnectionStrings, Is.EqualTo(ConfigurationManager.ConnectionStrings));
            Assert.That(NConfigurator.Default.AppSettings, Is.EqualTo(ConfigurationManager.AppSettings));
        }

        [Test]
        public void Should_promote_custom_configuration_to_default()
        {
            try
            {
                NConfigurator.UsingFile("Configs//NConfigTest.config").PromoteToDefault();

                Assert.That(NConfigurator.Default.FileNames, Is.EqualTo(NConfigurator.UsingFile("Configs//NConfigTest.config").FileNames));
                Assert.That(NConfigurator.Default.ConnectionStrings, Is.EqualTo(NConfigurator.UsingFile("Configs//NConfigTest.config").ConnectionStrings));
                Assert.That(NConfigurator.Default.AppSettings, Is.EqualTo(NConfigurator.UsingFile("Configs//NConfigTest.config").AppSettings));
            }
            finally
            {
                NConfigurator.UsingFiles().PromoteToDefault(); // Restore default Default
            }

        }

        [Test]
        public void Should_promote_custom_configuration_to_system_default()
        {
            try
            {
                var connectStrings = NConfigurator.UsingFile("Configs//NConfigTest.config").ConnectionStrings;
                var appSettings = NConfigurator.UsingFile("Configs//NConfigTest.config").AppSettings;
                NConfigurator.UsingFile("Configs//NConfigTest.config").PromoteToSystemDefault();

                Assert.That(ConfigurationManager.ConnectionStrings, Is.EqualTo(connectStrings));
                Assert.That(ConfigurationManager.AppSettings, Is.EqualTo(appSettings));
            }
            finally
            {
                NConfigurator.RestoreSystemDefaults();
            }
        }

        [Test]
        public void Should_return_non_requied_section_instance()
        {
            var section = NConfigurator.UsingFile("Configs//NConfigTest.config").GetSection("system.diagnostics");

            Assert.That(section, Is.Not.Null);
        }


    }
}
