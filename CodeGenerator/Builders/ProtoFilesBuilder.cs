﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeGenerator.Enums;

namespace CodeGenerator.Builders
{
    public sealed class ProtoFilesBuilder
    {
        private static readonly IReadOnlyCollection<string> HeaderLines = new[]
        {
            "syntax = \"proto3\";",
            "",
        };

        private static readonly IReadOnlyCollection<string> ServiceSubHeaderLines = new[]
        {
            "import \"google/protobuf/empty.proto\";",
            "",
        };

        private const string EmptyMessageType = "google.protobuf.Empty";

        private readonly List<MBApiMethodDefinition> _methods = new();
        private string _exportPath = string.Empty;
        private string _exportPathInsideProject = string.Empty;
        private string _returnParameterName = "response";
        private string _requestPostfix = "_Req";
        private string _responsePostfix = "_Res";
        private ServiceGenerationMode _serviceGenerationMode;

        public ProtoFilesBuilder AddMethods(IEnumerable<MBApiMethodDefinition> methods)
        {
            _methods.AddRange(methods);
            return this;
        }

        public ProtoFilesBuilder SetExportPath(string projectPath, string pathInsideProject)
        {
            _exportPath = Path.Combine(projectPath, pathInsideProject);
            _exportPathInsideProject = pathInsideProject;
            return this;
        }

        public ProtoFilesBuilder SetReturnParameterName(string returnParameterName)
        {
            _returnParameterName = returnParameterName;
            return this;
        }

        public ProtoFilesBuilder SetPostfixes(string requestPostfix, string responsePostfix)
        {
            _requestPostfix = requestPostfix;
            _responsePostfix = responsePostfix;
            return this;
        }

        public ProtoFilesBuilder SetServiceGenerationMode(ServiceGenerationMode serviceGenerationMode)
        {
            _serviceGenerationMode = serviceGenerationMode;
            return this;
        }

        public ProtoFilesBuilder DeleteCurrentProtoFiles()
        {
            var filesToDelete = new DirectoryInfo(_exportPath)
                .GetFiles()
                .Where(x => x.Extension.ToLower() == ".proto");

            foreach (var fileInfo in filesToDelete)
            {
                fileInfo.Delete();
            }

            return this;
        }

        public ProtoFilesBuilder CreateMessagesProtoFiles()
        {
            foreach (var methodDefinition in _methods)
            {
                CreateMessagesProtoFile(methodDefinition);
            }

            return this;
        }

        private void CreateMessagesProtoFile(MBApiMethodDefinition methodDefinition)
        {
            if (!methodDefinition.HasAnyParameters())
            {
                return;
            }

            var filePath = GetFilePath($"{methodDefinition.Name}.proto");

            var lines = HeaderLines
                .Concat(ToRequestLines(methodDefinition))
                .Concat(ToResponseLines(methodDefinition));

            File.WriteAllLines(filePath, lines);
        }

        public ProtoFilesBuilder CreateServiceProtoFile(string serviceName)
        {
            var filePath = GetFilePath($"{serviceName}.proto");

            var lines = _serviceGenerationMode switch
            {
                ServiceGenerationMode.MessagesInSeparateFiles => HeaderLines
                    .Concat(ServiceSubHeaderLines)
                    .Concat(GetServiceImportLines())
                    .Append(string.Empty)
                    .Concat(GetServiceLines(serviceName)),
                ServiceGenerationMode.SingleFile => HeaderLines
                    .Concat(ServiceSubHeaderLines)
                    .Concat(GetServiceLines(serviceName))
                    .Append(string.Empty)
                    .Concat(GetAllMessagesLines()),
                _ => throw new ArgumentOutOfRangeException(nameof(_serviceGenerationMode), _serviceGenerationMode, null)
            };

            File.WriteAllLines(filePath, lines);

            return this;
        }

        private IEnumerable<string> GetServiceImportLines()
        {
            return _methods
                .Where(x => x.HasAnyParameters())
                .Select(methodDefinition => $"import \"{_exportPathInsideProject}/{methodDefinition.Name}.proto\";");
        }

        private IEnumerable<string> GetServiceLines(string serviceName)
        {
            yield return $"service {serviceName} {{";

            foreach (var method in _methods)
            {
                var methodName = method.Name;
                var requestMessageType = GetRequestMessageType(method);
                var responseMessageType = GetResponseMessageType(method);
                yield return $"  rpc {methodName}({requestMessageType})";
                yield return $"      returns({responseMessageType});";
            }

            yield return "}";
        }

        private IEnumerable<string> GetAllMessagesLines()
        {
            var lines = Enumerable.Empty<string>();

            foreach (var method in _methods)
            {
                lines = lines.Concat(ToRequestLines(method));
                lines = lines.Concat(ToResponseLines(method));
            }

            return lines;
        }

        private IEnumerable<string> ToRequestLines(MBApiMethodDefinition methodDefinition)
        {
            if (!methodDefinition.HasInputParameters())
            {
                yield break;
            }

            yield return $"message {methodDefinition.Name}{_requestPostfix} {{";

            var parameterLines = methodDefinition.InputParameters
                .Select((x, i) => $"  {GetProtoType(x.Type)} {x.Name} = {i + 1};");
            foreach (var parameterLine in parameterLines)
            {
                yield return parameterLine;
            }

            yield return "}";
            yield return string.Empty;
        }

        private IEnumerable<string> ToResponseLines(MBApiMethodDefinition methodDefinition)
        {
            if (!methodDefinition.HasAnyOutputParameters())
            {
                yield break;
            }

            yield return $"message {methodDefinition.Name}{_responsePostfix} {{";

            var parametersIndexShift = 1;
            if (methodDefinition.HasReturnType())
            {
                parametersIndexShift++;
                yield return $"  {GetProtoType(methodDefinition.ReturnParameter.Type)} {_returnParameterName} = 1;";
            }

            var parameterLines = methodDefinition.OutputParameters
                .Select((x, i) => $"  {GetProtoType(x.Type)} {x.Name} = {i + parametersIndexShift};");
            foreach (var parameterLine in parameterLines)
            {
                yield return parameterLine;
            }

            yield return "}";
            yield return string.Empty;
        }

        private string GetFilePath(string fileName)
        {
            return !string.IsNullOrEmpty(_exportPath)
                ? Path.Combine(_exportPath, fileName)
                : fileName;
        }
    }
}