﻿using System;
using System.Collections.Generic;
using HotChocolate.Language;

namespace HotChocolate.Stitching
{
    public static class MergeSyntaxNodeExtensions
    {
        public static NameString CreateUniqueName(
            this ITypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            return $"{typeInfo.Schema.Name}_{typeInfo.Definition.Name.Value}";
        }

        public static NameString CreateUniqueName(
            this ITypeInfo typeInfo, NamedSyntaxNode namedSyntaxNode)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }

            if (namedSyntaxNode == null)
            {
                throw new ArgumentNullException(nameof(namedSyntaxNode));
            }

            return $"{typeInfo.Schema.Name}_{namedSyntaxNode.Name.Value}";
        }

        public static EnumTypeDefinitionNode Rename(
            this EnumTypeDefinitionNode enumTypeDefinition,
            NameString newName)
        {
            if (enumTypeDefinition == null)
            {
                throw new ArgumentNullException(nameof(enumTypeDefinition));
            }

            newName.EnsureNotEmpty(nameof(newName));

            NameString originalName = enumTypeDefinition.Name.Value;

            IReadOnlyList<DirectiveNode> directives =
                AddRenamedDirective(
                    enumTypeDefinition.Directives,
                    originalName);

            return enumTypeDefinition
                .WithName(new NameNode(newName))
                .WithDirectives(directives);
        }

        public static UnionTypeDefinitionNode Rename(
            this UnionTypeDefinitionNode enumTypeDefinition,
            NameString newName)
        {
            if (enumTypeDefinition == null)
            {
                throw new ArgumentNullException(nameof(enumTypeDefinition));
            }

            newName.EnsureNotEmpty(nameof(newName));

            NameString originalName = enumTypeDefinition.Name.Value;

            IReadOnlyList<DirectiveNode> directives =
                AddRenamedDirective(
                    enumTypeDefinition.Directives,
                    originalName);

            return enumTypeDefinition
                .WithName(new NameNode(newName))
                .WithDirectives(directives);
        }

        private static IReadOnlyList<DirectiveNode> AddRenamedDirective(
            IReadOnlyList<DirectiveNode> directives,
            NameString originalName)
        {
            var list = new List<DirectiveNode>(directives);

            list.RemoveAll(t =>
                DirectiveNames.Renamed.Equals(t.Name.Value));

            list.Add(new DirectiveNode
            (
                DirectiveNames.Renamed,
                new ArgumentNode(
                    DirectiveFieldNames.Renamed_Name,
                    originalName)
            ));

            return list;
        }

        public static FieldDefinitionNode AddDelegationPath(
            this FieldDefinitionNode field,
            NameString schemaName) =>
            AddDelegationPath(field, schemaName, null);

        public static FieldDefinitionNode AddDelegationPath(
            this FieldDefinitionNode field,
            NameString schemaName,
            string delegationPath)
        {
            if (field == null)
            {
                throw new ArgumentNullException(nameof(field));
            }

            schemaName.EnsureNotEmpty(nameof(schemaName));

            var list = new List<DirectiveNode>(field.Directives);

            list.RemoveAll(t =>
                DirectiveNames.Delegate.Equals(t.Name.Value));

            list.RemoveAll(t =>
                DirectiveNames.Schema.Equals(t.Name.Value));

            if (string.IsNullOrEmpty(delegationPath))
            {
                list.Add(new DirectiveNode(DirectiveNames.Delegate));
            }
            else
            {
                list.Add(new DirectiveNode
                (
                    DirectiveNames.Delegate,
                    new ArgumentNode(
                        DirectiveFieldNames.Delegate_Path,
                        delegationPath)
                ));
            }

            list.Add(new DirectiveNode
            (
                DirectiveNames.Schema,
                new ArgumentNode(
                    DirectiveFieldNames.Delegate_Path,
                    delegationPath)
            ));

            return field.WithDirectives(list);
        }
    }
}