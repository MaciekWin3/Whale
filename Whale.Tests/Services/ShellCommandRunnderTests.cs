using FluentAssertions;
using NUnit.Framework;
using System.Text.Json;
using Whale.Objects.Container;
using Whale.Services;

namespace Whale.Tests.Services
{
    [TestFixture]
    public class ShellCommandRunnderTests
    {
        [Test]
        public async Task ShouldReturnDotnetInfoOutput()
        {
            // Arrange
            var command = "dotnet";
            var arguments = new[] { "--info" };

            // Act
            var service = CreateShellCommandRunner();
            var result = await service.RunCommandAsync(command, arguments);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.std.ToString()
                .Substring(0, 4)
                .Should()
                .Be(".NET");
        }

        [Test]
        [Platform("Win")]
        public async Task ShouldReturnCmdSuccess()
        {
            // Arrange
            var command = "cmd";
            var arguments = new[] { "/C", "echo", "hello" };

            // Act
            var service = CreateShellCommandRunner();
            var result = await service.RunCommandAsync(command, arguments);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.std.ToString()
                .Trim()
                .Should()
                .Be("hello");
        }

        [Test]
        public async Task SHould()
        {
            var jsonText = @"{
        ""Id"": ""a6e319bfe8b7418ca9dbf65217e3d5e3d1d64dfc6a7984566d4bed4d434e729b"",
        ""Created"": ""2023-05-28T17:46:59.861857836Z"",
        ""Path"": ""bash"",
        ""Args"": [],
        ""State"": {
            ""Status"": ""exited"",
            ""Running"": false,
            ""Paused"": false,
            ""Restarting"": false,
            ""OOMKilled"": false,
            ""Dead"": false,
            ""Pid"": 0,
            ""ExitCode"": 255,
            ""Error"": """",
            ""StartedAt"": ""2023-05-28T17:47:00.236661764Z"",
            ""FinishedAt"": ""2023-05-29T19:43:18.567096716Z""
        },
        ""Image"": ""sha256:3b418d7b466ac6275a6bfcb0c86fbe4422ff6ea0af444a294f82d3bf5173ce74"",
        ""ResolvConfPath"": ""/var/lib/docker/containers/a6e319bfe8b7418ca9dbf65217e3d5e3d1d64dfc6a7984566d4bed4d434e729b/resolv.conf"",
        ""HostnamePath"": ""/var/lib/docker/containers/a6e319bfe8b7418ca9dbf65217e3d5e3d1d64dfc6a7984566d4bed4d434e729b/hostname"",
        ""HostsPath"": ""/var/lib/docker/containers/a6e319bfe8b7418ca9dbf65217e3d5e3d1d64dfc6a7984566d4bed4d434e729b/hosts"",
        ""LogPath"": ""/var/lib/docker/containers/a6e319bfe8b7418ca9dbf65217e3d5e3d1d64dfc6a7984566d4bed4d434e729b/a6e319bfe8b7418ca9dbf65217e3d5e3d1d64dfc6a7984566d4bed4d434e729b-json.log"",
        ""Name"": ""/sleepy_galileo"",
        ""RestartCount"": 0,
        ""Driver"": ""overlay2"",
        ""Platform"": ""linux"",
        ""MountLabel"": """",
        ""ProcessLabel"": """",
        ""AppArmorProfile"": """",
        ""ExecIDs"": null,
        ""HostConfig"": {
            ""Binds"": null,
            ""ContainerIDFile"": """",
            ""LogConfig"": {
                ""Type"": ""json-file"",
                ""Config"": {}
            },
            ""NetworkMode"": ""default"",
            ""PortBindings"": {},
            ""RestartPolicy"": {
                ""Name"": ""no"",
                ""MaximumRetryCount"": 0
            },
            ""AutoRemove"": false,
            ""VolumeDriver"": """",
            ""VolumesFrom"": null,
            ""CapAdd"": null,
            ""CapDrop"": null,
            ""CgroupnsMode"": ""host"",
            ""Dns"": [],
            ""DnsOptions"": [],
            ""DnsSearch"": [],
            ""ExtraHosts"": null,
            ""GroupAdd"": null,
            ""IpcMode"": ""private"",
            ""Cgroup"": """",
            ""Links"": null,
            ""OomScoreAdj"": 0,
            ""PidMode"": """",
            ""Privileged"": false,
            ""PublishAllPorts"": false,
            ""ReadonlyRootfs"": false,
            ""SecurityOpt"": null,
            ""UTSMode"": """",
            ""UsernsMode"": """",
            ""ShmSize"": 67108864,
            ""Runtime"": ""runc"",
            ""ConsoleSize"": [
                30,
                120
            ],
            ""Isolation"": """",
            ""CpuShares"": 0,
            ""Memory"": 0,
            ""NanoCpus"": 0,
            ""CgroupParent"": """",
            ""BlkioWeight"": 0,
            ""BlkioWeightDevice"": [],
            ""BlkioDeviceReadBps"": null,
            ""BlkioDeviceWriteBps"": null,
            ""BlkioDeviceReadIOps"": null,
            ""BlkioDeviceWriteIOps"": null,
            ""CpuPeriod"": 0,
            ""CpuQuota"": 0,
            ""CpuRealtimePeriod"": 0,
            ""CpuRealtimeRuntime"": 0,
            ""CpusetCpus"": """",
            ""CpusetMems"": """",
            ""Devices"": [],
            ""DeviceCgroupRules"": null,
            ""DeviceRequests"": null,
            ""KernelMemory"": 0,
            ""KernelMemoryTCP"": 0,
            ""MemoryReservation"": 0,
            ""MemorySwap"": 0,
            ""MemorySwappiness"": null,
            ""OomKillDisable"": false,
            ""PidsLimit"": null,
            ""Ulimits"": null,
            ""CpuCount"": 0,
            ""CpuPercent"": 0,
            ""IOMaximumIOps"": 0,
            ""IOMaximumBandwidth"": 0,
            ""MaskedPaths"": [
                ""/proc/asound"",
                ""/proc/acpi"",
                ""/proc/kcore"",
                ""/proc/keys"",
                ""/proc/latency_stats"",
                ""/proc/timer_list"",
                ""/proc/timer_stats"",
                ""/proc/sched_debug"",
                ""/proc/scsi"",
                ""/sys/firmware""
            ],
            ""ReadonlyPaths"": [
                ""/proc/bus"",
                ""/proc/fs"",
                ""/proc/irq"",
                ""/proc/sys"",
                ""/proc/sysrq-trigger""
            ]
        },
        ""GraphDriver"": {
            ""Data"": {
                ""LowerDir"": ""/var/lib/docker/overlay2/3ea9a869f1dc497c882c21f1cb57a869a385835da5e3ba872e968fd5600bf1d2-init/diff:/var/lib/docker/overlay2/1baa80bea5bb948aea24901dcee7a3ce66a5c42cf3045e1819ad721d715ab14e/diff"",
                ""MergedDir"": ""/var/lib/docker/overlay2/3ea9a869f1dc497c882c21f1cb57a869a385835da5e3ba872e968fd5600bf1d2/merged"",
                ""UpperDir"": ""/var/lib/docker/overlay2/3ea9a869f1dc497c882c21f1cb57a869a385835da5e3ba872e968fd5600bf1d2/diff"",
                ""WorkDir"": ""/var/lib/docker/overlay2/3ea9a869f1dc497c882c21f1cb57a869a385835da5e3ba872e968fd5600bf1d2/work""
            },
            ""Name"": ""overlay2""
        },
        ""Mounts"": [],
        ""Config"": {
            ""Hostname"": ""a6e319bfe8b7"",
            ""Domainname"": """",
            ""User"": """",
            ""AttachStdin"": true,
            ""AttachStdout"": true,
            ""AttachStderr"": true,
            ""Tty"": true,
            ""OpenStdin"": true,
            ""StdinOnce"": true,
            ""Env"": [
                ""PATH=/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin""
            ],
            ""Cmd"": [
                ""bash""
            ],
            ""Image"": ""ubuntu"",
            ""Volumes"": null,
            ""WorkingDir"": """",
            ""Entrypoint"": null,
            ""OnBuild"": null,
            ""Labels"": {
                ""org.opencontainers.image.ref.name"": ""ubuntu"",
                ""org.opencontainers.image.version"": ""22.04""
            }
        },
        ""NetworkSettings"": {
            ""Bridge"": """",
            ""SandboxID"": ""44d2dcbd193cc69952f4d2c8d22617691e8874cc424c5e30655b1551994ceb3e"",
            ""HairpinMode"": false,
            ""LinkLocalIPv6Address"": """",
            ""LinkLocalIPv6PrefixLen"": 0,
            ""Ports"": {},
            ""SandboxKey"": ""/var/run/docker/netns/44d2dcbd193c"",
            ""SecondaryIPAddresses"": null,
            ""SecondaryIPv6Addresses"": null,
            ""EndpointID"": ""6eca7821af416bb15179c3630fbdbb0d2a6aa3912cf74d44771de6fb66f0b07a"",
            ""Gateway"": ""172.17.0.1"",
            ""GlobalIPv6Address"": """",
            ""GlobalIPv6PrefixLen"": 0,
            ""IPAddress"": ""172.17.0.2"",
            ""IPPrefixLen"": 16,
            ""IPv6Gateway"": """",
            ""MacAddress"": ""02:42:ac:11:00:02"",
            ""Networks"": {
                ""bridge"": {
                    ""IPAMConfig"": null,
                    ""Links"": null,
                    ""Aliases"": null,
                    ""NetworkID"": ""4fc1c202bd2c9ad6a89174942f0aea8275bd79c4c5c8da2d46cadd44da3d68c7"",
                    ""EndpointID"": ""6eca7821af416bb15179c3630fbdbb0d2a6aa3912cf74d44771de6fb66f0b07a"",
                    ""Gateway"": ""172.17.0.1"",
                    ""IPAddress"": ""172.17.0.2"",
                    ""IPPrefixLen"": 16,
                    ""IPv6Gateway"": """",
                    ""GlobalIPv6Address"": """",
                    ""GlobalIPv6PrefixLen"": 0,
                    ""MacAddress"": ""02:42:ac:11:00:02"",
                    ""DriverOpts"": null
                }
            }
        }
    }
";

            var z = JsonSerializer.Deserialize<Container>(jsonText, new JsonSerializerOptions()
            {
            });

            z.Should().NotBeNull();
            z.Should().BeOfType<Container>();
        }

        private ShellCommandRunner CreateShellCommandRunner()
        {
            return new ShellCommandRunner();
        }
    }
}
