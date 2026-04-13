using System;
using System.Collections.Generic;

namespace IndependentWork22
{
    // ================= COMPONENT =================
    public interface IComponent
    {
        void Display(int indent);
    }

    // Базовий клас для зручної композиції декораторів
    public abstract class FileSystemComponent : IComponent
    {
        public abstract string Name { get; }
        public void Display(int indent) => DisplayInternal(indent, n => n);

        // transform застосовується лише до імені поточного вузла
        internal abstract void DisplayInternal(int indent, Func<string, string> transform);
    }

    // ================= LEAF =================
    public sealed class FileItem : FileSystemComponent
    {
        public override string Name { get; }
        public long Size { get; }

        public FileItem(string name, long size)
        {
            Name = name;
            Size = size;
        }

        internal override void DisplayInternal(int indent, Func<string, string> transform)
        {
            Console.WriteLine($"{new string(' ', indent)}- File: {transform(Name)} ({Size} KB)");
        }
    }

    // ================= COMPOSITE =================
    public sealed class Folder : FileSystemComponent
    {
        private readonly List<IComponent> _children = new();

        public override string Name { get; }

        public Folder(string name)
        {
            Name = name;
        }

        public void Add(IComponent component) => _children.Add(component);
        public void Remove(IComponent component) => _children.Remove(component);

        internal override void DisplayInternal(int indent, Func<string, string> transform)
        {
            Console.WriteLine($"{new string(' ', indent)}+ Folder: {transform(Name)}");

            foreach (var child in _children)
            {
                if (child is FileSystemComponent fs)
                    fs.DisplayInternal(indent + 2, n => n);
                else
                    child.Display(indent + 2);
            }
        }
    }

    // ================= DECORATOR =================
    public abstract class ComponentDecorator : FileSystemComponent
    {
        protected readonly IComponent _component;

        protected ComponentDecorator(IComponent component)
        {
            _component = component;
        }

        public override string Name =>
            _component is FileSystemComponent fs ? fs.Name : "Component";

        protected abstract string DecorateName(string original);

        internal override void DisplayInternal(int indent, Func<string, string> transform)
        {
            Func<string, string> composed = n => transform(DecorateName(n));

            if (_component is FileSystemComponent fs)
                fs.DisplayInternal(indent, composed);
            else
                _component.Display(indent);
        }
    }

    public sealed class ReadOnlyDecorator : ComponentDecorator
    {
        public ReadOnlyDecorator(IComponent component) : base(component) { }
        protected override string DecorateName(string original) => $"[R] {original}";
    }

    public sealed class CompressedDecorator : ComponentDecorator
    {
        public CompressedDecorator(IComponent component) : base(component) { }
        protected override string DecorateName(string original) => $"{original}[.zip]";
    }

    // ================= MAIN =================
    internal class Program
    {
        static void Main(string[] args)
        {
            // Leaf
            var file1 = new FileItem("report.docx", 120);
            var file2 = new FileItem("photo.png", 2048);
            var file3 = new FileItem("notes.txt", 15);

            // Composite
            var docs = new Folder("Documents");
            docs.Add(file1);
            docs.Add(file3);

            var images = new Folder("Images");
            images.Add(file2);

            var root = new Folder("Root");
            root.Add(docs);
            root.Add(images);

            // Декоровані об’єкти
            IComponent readOnlyFile = new ReadOnlyDecorator(file1);
            IComponent compressedFile = new CompressedDecorator(file2);
            IComponent readOnlyCompressedFile = new CompressedDecorator(new ReadOnlyDecorator(file3));

            IComponent readOnlyFolder = new ReadOnlyDecorator(docs);
            IComponent compressedFolder = new CompressedDecorator(images);
            IComponent readOnlyCompressedFolder = new CompressedDecorator(new ReadOnlyDecorator(root));

            Console.WriteLine("=== Недекороване дерево ===");
            root.Display(0);

            Console.WriteLine("\n=== Декоровані Leaf ===");
            readOnlyFile.Display(0);
            compressedFile.Display(0);
            readOnlyCompressedFile.Display(0);

            Console.WriteLine("\n=== Декоровані Composite ===");
            readOnlyFolder.Display(0);
            compressedFolder.Display(0);

            Console.WriteLine("\n=== Комбінація декораторів на всьому дереві ===");
            readOnlyCompressedFolder.Display(0);
        }
    }
}