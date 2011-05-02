﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ItzWarty;
using ItzWarty.RAF;

using System.Windows.Forms;
namespace RAF_Packer
{
    /// <summary>
    /// Enumeration defines the type of FSO we have.
    /// </summary>
    public enum RAFFSOType
    {
        DIRECTORY,
        FILE,
        ARCHIVE
    }
    public class RAFInMemoryFileSystemObject : TreeNode
    {
        private RAFFSOType fsoType;
        public RAFInMemoryFileSystemObject(RAFInMemoryFileSystemObject parent, RAFFSOType fsoType, string name)
            :base( //lol.  Is this a sign that I haven't played with C# enough?
                (fsoType==RAFFSOType.DIRECTORY?
                    "[DIR] ":
                    (fsoType==RAFFSOType.ARCHIVE?
                        "[ARC] ":
                        "[FIL] "
                    )
                ) +  name
            )
        {
            //if(this.parent != null)
            //    this.parent.Nodes.Add(this);

            this.fsoType = fsoType;
            this.Name = name;
        }
        /// <summary>
        /// Gets a child FSO of the given name
        /// </summary>
        public RAFInMemoryFileSystemObject GetChildFSO(string name)
        {
            foreach (RAFInMemoryFileSystemObject rafFso in this.Nodes)
            {
                if (rafFso.Name.ToLower() == name.ToLower())
                    return rafFso;
            }
            return null;
        }
        /// <summary>
        /// Adds a child FSO to this node
        /// </summary>
        public RAFInMemoryFileSystemObject AddChildFSO(RAFFSOType type, string name)
        {
            RAFInMemoryFileSystemObject result = null;
            this.Nodes.Add(
                result = new RAFInMemoryFileSystemObject(
                    this, type, name
                )
            );
            return result;
        }
        /// <summary>
        /// Adds a child FSO to this treenode.  
        /// This function supports pathing, so you can add a file with directory names!
        /// 
        /// Directories are created if not existing already
        /// 
        /// Such as: root/subDir/SubSubDir/file.name
        /// </summary>
        public RAFInMemoryFileSystemObject AddToTree(RAFFSOType type, string name)
        {
            string[] dirNames = name.Replace("\\", "/").Split("/");

            RAFInMemoryFileSystemObject currentNode = this;

            //Traverse FS Tree to the directory containing our file
            for(int i = 0; i < dirNames.Length-1; i++)
            {
                RAFInMemoryFileSystemObject childNode = currentNode.GetChildFSO(dirNames[i]);

                if (childNode == null)
                    childNode = currentNode.AddChildFSO(RAFFSOType.DIRECTORY, dirNames[i]);

                currentNode = childNode;
            }

            //Add the childnode to our tree..
            return currentNode.AddChildFSO(RAFFSOType.FILE, dirNames.Last());
        }
        /// <summary>
        /// Gets the full path of this FSO in the Riot Archive File.
        /// </summary>
        public string GetRAFPath()
        {
            Stack<string> resultStack = new Stack<string>();
            RAFInMemoryFileSystemObject currentNode = this;
            while (currentNode != null && currentNode.GetFSOType() != RAFFSOType.ARCHIVE)
            {
                resultStack.Push(currentNode.Name);
                currentNode = (RAFInMemoryFileSystemObject)currentNode.Parent;
            }

            if (resultStack.Count == 0) return "";
            //flatten the stack
            StringBuilder resultSb = new StringBuilder();
            resultSb.Append(resultStack.Pop());
            while (resultStack.Count != 0)
                resultSb.Append("/"+resultStack.Pop());

            return resultSb.ToString();
        }

        public RAFFSOType GetFSOType()
        {
            return this.fsoType;
        }
        public RAFInMemoryFileSystemObject GetTopmostParent()
        {
            RAFInMemoryFileSystemObject currentNode = this;
            while (currentNode.Parent != null)
                currentNode = (RAFInMemoryFileSystemObject)currentNode.Parent;
            return currentNode;
        }
    }
}
