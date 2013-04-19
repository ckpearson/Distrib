/*
	This software known as 'Distrib' is under the GNU GPL v2. License
		This license can be found at: http://www.gnu.org/licenses/gpl-2.0.html

	Unless this software has been made available to you under the terms of an alternative license by
	Clint Pearson, your use of this software is dependent upon compliance with the GNU GPL v2. license.

	This software is the sole copyright of Clint Pearson
		Contact: clintkpearson@gmail.com

	This software is provided with NO WARRANTY at all, explicit or implied.

	If you wish to contact me about the software / licensing you can reach me at distribgrid@gmail.com
*/
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Distrib.Storage
{
    /// <summary>
    /// Provides persistence services for XML sources and targets
    /// </summary>
    public static class XmlPersistor
    {

        /// <summary>
        /// Persists a given <see cref="IPersist"/>-aware instance to a file as XML
        /// </summary>
        /// <param name="path">The file path</param>
        /// <param name="persistable">The <see cref="IPersist"/> instance</param>
        /// <param name="mode">The <see cref="FileMode"/> to use</param>
        /// <param name="indent">Whether to indent the XML</param>
        public static void Persist(string path, IPersist persistable, FileMode mode = FileMode.CreateNew, bool indent = true)
        {
            if (string.IsNullOrEmpty(path)) throw Ex.ArgNull(() => path);
            if (persistable == null) throw Ex.ArgNull(() => persistable);

            try
            {
                using (var fs = new FileStream(
                        path,
                        mode,
                        FileAccess.Write,
                        FileShare.Read))
                {
                    Persist(fs, persistable, indent);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to persist to file", ex);
            }
        }

        /// <summary>
        /// Persists a given <see cref="IPersist"/> to a <see cref="Stream"/> as XML
        /// </summary>
        /// <param name="stream">The stream to persist to</param>
        /// <param name="persistable">The <see cref="IPersist"/>-aware instance</param>
        /// <param name="indent">Whether to indent the XML</param>
        public static void Persist(Stream stream, IPersist persistable, bool indent = true)
        {
            if (stream == null) throw Ex.ArgNull(() => stream);
            if (persistable == null) throw Ex.ArgNull(() => persistable);

            if (!stream.CanWrite) throw Ex.Arg(() => stream, "Stream must be writable");

            try
            {
                Persist(XmlWriter.Create(stream, new XmlWriterSettings()
                        {
                            Indent = indent,
                            OmitXmlDeclaration = true,
                        }), persistable);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to persist to stream", ex);
            }
        }

        /// <summary>
        /// Persists a given <see cref="IPersist"/> to an <see cref="XmlWriter"/>
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> to persist to</param>
        /// <param name="persistable">The <see cref="IPersist"/>-aware instance</param>
        public static void Persist(XmlWriter writer, IPersist persistable)
        {
            if (writer == null) throw Ex.ArgNull(() => writer);
            if (persistable == null) throw Ex.ArgNull(() => persistable);

            try
            {
                var xser = new XmlSerializer(typeof(PersistRecords));
                xser.Serialize(writer, PersistHelper.GetRecordsFromPersistable(persistable));
                writer.Flush();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to persist to XmlWriter", ex);
            }
        }

        /// <summary>
        /// Loads the <see cref="PersistRecords"/> from XML to be read from the given file
        /// </summary>
        /// <param name="path">The path to the file</param>
        /// <returns>The read <see cref="PersistRecords"/></returns>
        public static PersistRecords LoadRecords(string path)
        {
            if (string.IsNullOrEmpty(path)) throw Ex.ArgNull(() => path);

            try
            {
                using (var fs = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read))
                {
                    return LoadRecords(fs);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load records from file", ex);
            }
        }

        /// <summary>
        /// Loads the <see cref="PersistRecords"/> from XML to be read from the given <see cref="Stream"/>
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> to read from</param>
        /// <returns>The read <see cref="PersistRecords"/></returns>
        public static PersistRecords LoadRecords(Stream stream)
        {
            if (stream == null) throw Ex.ArgNull(() => stream);
            if (!stream.CanRead) throw Ex.Arg(() => stream, "Stream must be readable");

            try
            {
                return LoadRecords(XmlReader.Create(stream, new XmlReaderSettings()
                        {

                        }));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load records from stream", ex);
            }
        }

        /// <summary>
        /// Loads the <see cref="PersistRecords"/> from a given <see cref="XmlReader"/>
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> to load the records from</param>
        /// <returns>The read <see cref="PersistRecords"/></returns>
        public static PersistRecords LoadRecords(XmlReader reader)
        {
            if (reader == null) throw Ex.ArgNull(() => reader);

            try
            {
                var xser = new XmlSerializer(typeof(PersistRecords));
                return (PersistRecords)xser.Deserialize(reader);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load records from XmlReader", ex);
            }
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> from the <see cref="PersistRecords"/> read from the XML file
        /// </summary>
        /// <typeparam name="T">The instance type</typeparam>
        /// <param name="path">The path to the file</param>
        /// <returns>The created instance initialised with the persisted data</returns>
        public static T LoadInstance<T>(string path) where T : IPersist
        {
            if (string.IsNullOrEmpty(path)) throw Ex.ArgNull(() => path);

            try
            {
                using (var fs = new FileStream(
                    path,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read))
                {
                    return LoadInstance<T>(fs);
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load instance from file", ex);
            }
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> from the <see cref="PersistRecords"/> read from XML present in the <see cref="Stream"/>
        /// </summary>
        /// <typeparam name="T">The instance type</typeparam>
        /// <param name="stream">The <see cref="Stream"/> to read from</param>
        /// <returns>The created instance initialised with the persisted data</returns>
        public static T LoadInstance<T>(Stream stream) where T : IPersist
        {
            if (stream == null) throw Ex.ArgNull(() => stream);
            if (!stream.CanRead) throw Ex.Arg(() => stream, "Stream must be readable");

            try
            {
                return LoadInstance<T>(XmlReader.Create(stream,
                        new XmlReaderSettings()
                        {

                        }));
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load instance from stream", ex);
            }
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> from the <see cref="PersistRecords"/> read from the <see cref="XmlReader"/>
        /// </summary>
        /// <typeparam name="T">The instance type</typeparam>
        /// <param name="reader">The <see cref="XmlReader"/> to read from</param>
        /// <returns>The created instance initialised with the persisted data</returns>
        public static T LoadInstance<T>(XmlReader reader) where T : IPersist
        {
            if (reader == null) throw Ex.ArgNull(() => reader);

            try
            {
                var records = LoadRecords(reader);
                var inst = Activator.CreateInstance<T>();
                ((IPersist)inst).LoadFromPersisted(records);
                return inst;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to load instance from XmlReader", ex);
            }
        }
    }
}
