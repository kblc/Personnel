﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Personnel.Services.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Personnel.Services.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Bad identifier format for string &apos;{0}&apos;..
        /// </summary>
        public static string BASESERVICE_BadIdentifierFormat {
            get {
                return ResourceManager.GetString("BASESERVICE_BadIdentifierFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File with identifier &apos;{0}&apos; not found..
        /// </summary>
        public static string FILESERVICE_FileNotFound {
            get {
                return ResourceManager.GetString("FILESERVICE_FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Picture with file identifier &apos;{0}&apos; file already exists.
        /// </summary>
        public static string FILESERVICE_PictureAlreadyExists {
            get {
                return ResourceManager.GetString("FILESERVICE_PictureAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Appoint with identifier &apos;{0}&apos; not found..
        /// </summary>
        public static string STUFFINGSERVICE_AppointNotFound {
            get {
                return ResourceManager.GetString("STUFFINGSERVICE_AppointNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Department with identifier &apos;{0}&apos; not found..
        /// </summary>
        public static string STUFFINGSERVICE_DepartmentNotFound {
            get {
                return ResourceManager.GetString("STUFFINGSERVICE_DepartmentNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Employee with identifier &apos;{0} not found..
        /// </summary>
        public static string STUFFINGSERVICE_EmployeeNotFound {
            get {
                return ResourceManager.GetString("STUFFINGSERVICE_EmployeeNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Right with name &apos;{0}&apos; not found..
        /// </summary>
        public static string STUFFINGSERVICE_RightNotFound {
            get {
                return ResourceManager.GetString("STUFFINGSERVICE_RightNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Staffing with identifier &apos;{0}&apos; not found..
        /// </summary>
        public static string STUFFINGSERVICE_StaffingNotFound {
            get {
                return ResourceManager.GetString("STUFFINGSERVICE_StaffingNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User must have following rights: {0}..
        /// </summary>
        public static string STUFFINGSERVICE_UserHaveNoFollowingRights {
            get {
                return ResourceManager.GetString("STUFFINGSERVICE_UserHaveNoFollowingRights", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to User with name &apos;{0}&apos; not identified..
        /// </summary>
        public static string STUFFINGSERVICE_UserNotIdentified {
            get {
                return ResourceManager.GetString("STUFFINGSERVICE_UserNotIdentified", resourceCulture);
            }
        }
    }
}