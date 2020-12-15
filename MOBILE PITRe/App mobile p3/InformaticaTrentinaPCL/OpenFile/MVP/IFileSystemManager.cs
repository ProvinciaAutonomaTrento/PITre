using System;

namespace InformaticaTrentinaPCL.OpenFile.MVP
{
    public interface IFileSystemManager
    {
        void saveFileAndOpen(byte[] inputBytes, string fileName, string extension, Object extra);
    }
}