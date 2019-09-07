using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using DataVisualization.Services.Exceptions;

namespace DataVisualization.Services
{
    public class DataFileSaver
    {
        /// <summary>
        /// Saves string content to a file
        /// </summary>
        /// <exception cref="DataIOException">Thrown when failed to save file</exception>
        public async Task SaveAsync(string filePath, string content)
        {
            try
            {
                using (var file = File.OpenWrite(filePath))
                using (var stream = new StreamWriter(file))
                {
                    await stream.WriteAsync(content);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new DataIOException("You dont have access to this file", ex);
            }
            catch (PathTooLongException ex)
            {
                throw new DataIOException("Path to your file is too long for your operating system", ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new DataIOException("Path to your file does not exist", ex);
            }
        }
    }
}
