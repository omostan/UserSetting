using System.IO;
using System.Reflection;

namespace UserSetting.Controls.Properties;

public class CoreAssemblyInfo
{
    private readonly Assembly _assembly;

    /// <summary>
    /// Check and create an object of the assembly
    /// </summary>
    /// <param name="assembly"></param>
    public CoreAssemblyInfo(Assembly assembly)
    {
        if (assembly != null)
        {
            _assembly = assembly;
        }
        else
        {
            throw new ArgumentNullException(nameof(assembly));
        }
    }

    #region Assembly attributes

    /// <summary>
    /// Gets the Author's name and other attributes
    /// </summary>
    public string Author
    {
        get
        {
            return GetAttributeValue<AssemblyCustomAuthorAttribute>(
                a => a.Author, Path.GetFileNameWithoutExtension(_assembly.FullName));
        }
    }

    /// <summary>
    /// Gets the Author's company department
    /// </summary>
    public string Department
    {
        get
        {
            return GetAttributeValue<AssemblyCustomDepartmentAttribute>(
                d => d.Department, Path.GetFileNameWithoutExtension(_assembly.FullName));
        }
    }

    /// <summary>
    /// Gets the title property
    /// </summary>
    public string ProductTitle
    {
        get
        {
            return GetAttributeValue<AssemblyTitleAttribute>(
                a => a.Title, Path.GetFileNameWithoutExtension(_assembly.FullName));
        }
    }

    /// <summary>
    /// Gets the application's version
    /// </summary>
    public string Version
    {
        get
        {
            Version? version = _assembly.GetName().Version;
            return version != null ? "v" + version.ToString() : string.Empty;
        }
    }

    /// <summary>
    /// Gets the description about the application.
    /// </summary>
    public string Description
    {
        get { return GetAttributeValue<AssemblyDescriptionAttribute>(a => a.Description); }
    }


    /// <summary>
    ///  Gets the product's full name.
    /// </summary>
    public string Product
    {
        get { return GetAttributeValue<AssemblyProductAttribute>(a => a.Product); }
    }

    /// <summary>
    /// Gets the copyright information for the product.
    /// </summary>
    public string Copyright
    {
        get { return GetAttributeValue<AssemblyCopyrightAttribute>(a => a.Copyright); }
    }

    /// <summary>
    /// Gets the company information for the product.
    /// </summary>
    public string Company
    {
        get { return GetAttributeValue<AssemblyCompanyAttribute>(a => a.Company); }
    }

    protected string GetAttributeValue<TAttr>(Func<TAttr,
      string> resolveFunc, string? defaultResult = null) where TAttr : Attribute
    {
        object[] attributes = _assembly.GetCustomAttributes(typeof(TAttr), false);
        return attributes.Length > 0 ? resolveFunc((TAttr)attributes[0]) : defaultResult!;
    }

    #endregion
}

#region AssemblyInfo helper classes

/////////////////////////////////////////////////////////////////
///         Two additional AssemblyInfo helper classes          //
/////////////////////////////////////////////////////////////////

/// <summary>
/// This helper class allows the creation of additional custom
/// author attribute in die AssemblyInfo.cs
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyCustomAuthorAttribute : Attribute
{
    public virtual string Author { get; }
    public AssemblyCustomAuthorAttribute(string Author)
    {
        this.Author = Author;
    }
}

/// <summary>
/// his helper class allows the creation of additional custom
/// department author attribute in die AssemblyInfo.cs
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyCustomDepartmentAttribute : Attribute
{
    public virtual string Department { get; }
    public AssemblyCustomDepartmentAttribute(string Department)
    {
        this.Department = Department;
    }
}

#endregion
