Imports System
Imports System.Data
Imports System.Data.Entity
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Data.Entity.Core.Objects
Imports System.Web
Imports System.Web.Security
Imports CMS.Core.Common
Imports CMS.Core.ViewModels
Imports System.Data.Entity.Validation

'2016-02-11 泛型處理程式
'http://kevintsengtw.blogspot.tw/2012/10/aspnet-mvc-part2-repository.html

Namespace Repository
    Public Class GenericRepository(Of TEntity As Class)
        Implements IRepository(Of TEntity)

        Private _context As DbContext
        Private _CurrentUser As EmployeeViewModel = Nothing
        Private Function GetCurrentUser() As EmployeeViewModel
            If IsNothing(System.Web.HttpContext.Current.User) Then
                Throw New NullReferenceException("System.Web.HttpContext.Current.User")
            End If

            Dim u As FormsIdentity = CType(HttpContext.Current.User.Identity, FormsIdentity)
            Dim obj As Object = JsonHelper.JsonToObject(Of EmployeeViewModel)(u.Ticket.UserData)
            Dim result As EmployeeViewModel = CType(obj, EmployeeViewModel)
            Return result

            'If Not IsNothing(System.Web.HttpContext.Current.User) Then
            '    Return System.Web.HttpContext.Current.User.Identity.Name
            'Else
            '    Return CurrentUser.EmployeeID
            'End If
            'Return ""
        End Function


        '目前登入的使用者
        Protected ReadOnly Property CurrentUser As EmployeeViewModel
            Get
                If (IsNothing(_CurrentUser)) Then
                    _CurrentUser = GetCurrentUser()
                End If
                Return _CurrentUser
            End Get
        End Property
        Protected ReadOnly Property Path As String
            Get
                'Return System.Web.VirtualPathUtility.ToAbsolute("~/Source/CMReport.aspx")
                Return ""
            End Get
        End Property

        Protected ReadOnly Property Context As DbContext
            Get
                Return _context
            End Get
        End Property

        Public Sub New()
            Me.New(New CMSEntities())
        End Sub
        Public Sub New(context As DbContext)
            If IsNothing(context) Then
                Throw New ArgumentNullException("context")
            End If

            Me._context = context
            'Me._context.Database.Log = Sub(val) Diagnostics.Trace.WriteLine(val)
            Me._context.Database.Log = Sub(val) System.Diagnostics.Debug.Write(val)

        End Sub
        Public Sub New(context As ObjectContext)
            If IsNothing(context) Then
                Throw New ArgumentNullException("context")
            End If

            Me._context = New DbContext(context, True)
        End Sub

        'Public Sub Create(instance As TEntity) Implements IRepository(Of TEntity).Create
        '    If IsNothing(instance) Then
        '        Throw New ArgumentNullException("instance")
        '    Else
        '        Me._context.Set(Of TEntity)().Add(instance)
        '        Me.SaveChanges()
        '    End If
        'End Sub
        Public Function Create(instance As TEntity) As TEntity _
                Implements IRepository(Of TEntity).Create
            If IsNothing(instance) Then
                Throw New ArgumentNullException("instance")
            Else
                Me._context.Set(Of TEntity)().Add(instance)
                Me.SaveChanges()
                Return instance
            End If
        End Function

        Public Sub Delete(instance As TEntity) Implements IRepository(Of TEntity).Delete
            If IsNothing(instance) Then
                Throw New ArgumentNullException("instance")
            Else
                Me._context.Entry(instance).State = EntityState.Deleted
                Me.SaveChanges()
            End If
        End Sub

        Public Function GetAll() As IQueryable(Of TEntity) Implements IRepository(Of TEntity).GetAll
            Return Me._context.Set(Of TEntity)().AsQueryable()
        End Function

        Public Function GetOne(predicate As Expression(Of Func(Of TEntity, Boolean))) As TEntity Implements IRepository(Of TEntity).GetOne
            Return Me._context.Set(Of TEntity)().SingleOrDefault(predicate)
        End Function

        Public Sub SaveChanges() Implements IRepository(Of TEntity).SaveChanges
            Try
                Me._context.SaveChanges()
            Catch ex As DbEntityValidationException
                Dim errorMessages = ex.EntityValidationErrors _
                                   .SelectMany(Function(x) x.ValidationErrors) _
                                   .Select(Function(x) x.ErrorMessage)
                Dim fullErrorMessage = String.Join("; ", errorMessages)
                Dim exceptionMessage = String.Concat(ex.Message, " The validation errors are: ", fullErrorMessage)
                Throw New DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors)
            End Try
        End Sub

        'Public Sub Update(instance As TEntity) Implements IRepository(Of TEntity).Update
        '    If IsNothing(instance) Then
        '        Throw New ArgumentNullException("instance")
        '    Else
        '        Me._context.Entry(instance).State = EntityState.Modified
        '        Me.SaveChanges()
        '    End If
        'End Sub
        Public Function Update(instance As TEntity) As TEntity _
                Implements IRepository(Of TEntity).Update

            If IsNothing(instance) Then
                Throw New ArgumentNullException("instance")
            Else
                Me._context.Entry(instance).State = EntityState.Modified
                Me.SaveChanges()
                Return instance
            End If
        End Function

        Function AsQuerable() As IQueryable(Of TEntity) _
            Implements IRepository(Of TEntity).AsQuerable
            Dim result = Me._context.Set(Of TEntity)().AsQueryable()
            Return result
        End Function


#Region "C#"
        ' private DbContext _context
        '{
        '    get;
        '    set;
        '}

        'Public GenericRepository()
        '    : this(new TestDBEntities())
        '{
        '}

        'public GenericRepository(DbContext context)
        '{
        '    if (context == null)
        '    {
        '        throw new ArgumentNullException("context");
        '    }            
        '    this._context = context;
        '}

        'public GenericRepository(ObjectContext context)
        '{
        '    if (context == null)
        '    {
        '        throw new ArgumentNullException("context");
        '    }
        '    this._context = new DbContext(context, true);
        '}



        '/// <summary>
        '/// Creates the specified instance.
        '/// </summary>
        '/// <param name="instance">The instance.</param>
        '/// <exception cref="System.ArgumentNullException">instance</exception>
        'public void Create(TEntity instance)
        '{
        '    if (instance == null)
        '    {
        '        throw new ArgumentNullException("instance");
        '    }
        '    else
        '    {
        '        this._context.Set<TEntity>().Add(instance);
        '        this.SaveChanges();
        '    }

        '}

        '/// <summary>
        '/// Updates the specified instance.
        '/// </summary>
        '/// <param name="instance">The instance.</param>
        '/// <exception cref="System.NotImplementedException"></exception>
        'public void Update(TEntity instance)
        '{
        '    if (instance == null)
        '    {
        '        throw new ArgumentNullException("instance");
        '    }
        '    else
        '    {
        '        this._context.Entry(instance).State = EntityState.Modified;
        '        this.SaveChanges();
        '    }
        '}

        '/// <summary>
        '/// Deletes the specified instance.
        '/// </summary>
        '/// <param name="instance">The instance.</param>
        '/// <exception cref="System.NotImplementedException"></exception>
        'public void Delete(TEntity instance)
        '{
        '    if (instance == null)
        '    {
        '        throw new ArgumentNullException("instance");
        '    }
        '    else
        '    {
        '        this._context.Entry(instance).State = EntityState.Deleted;
        '        this.SaveChanges();
        '    }
        '}

        '/// <summary>
        '/// Gets the specified predicate.
        '/// </summary>
        '/// <param name="predicate">The predicate.</param>
        '/// <returns></returns>
        'public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        '{
        '    return this._context.Set<TEntity>().FirstOrDefault(predicate);
        '}

        '/// <summary>
        '/// Gets all.
        '/// </summary>
        '/// <returns></returns>
        'public IQueryable<TEntity> GetAll()
        '{
        '    return this._context.Set<TEntity>().AsQueryable();
        '}


        'public void SaveChanges()
        '{
        '    this._context.SaveChanges();
        '}

        'public void Dispose()
        '{
        '    this.Dispose(true);
        '    GC.SuppressFinalize(this);
        '}

        'protected virtual void Dispose(bool disposing)
        '{
        '    if (disposing)
        '    {
        '        if (this._context != null)
        '        {
        '            this._context.Dispose();
        '            this._context = null;
        '        }
        '    }
        '}
        '}
#End Region


#Region "IDisposable Support"
        Private disposedValue As Boolean ' 偵測多餘的呼叫

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO:  處置 Managed 狀態 (Managed 物件)。
                    If IsNothing(_context) = False Then
                        Me._context.Dispose()
                        Me._context = Nothing
                    End If
                End If

                ' TODO:  釋放 Unmanaged 資源 (Unmanaged 物件) 並覆寫下面的 Finalize()。
                ' TODO:  將大型欄位設定為 null。
            End If
            Me.disposedValue = True
        End Sub

        ' TODO:  只有當上面的 Dispose(ByVal disposing As Boolean) 有可釋放 Unmanaged 資源的程式碼時，才覆寫 Finalize()。
        'Protected Overrides Sub Finalize()
        '    ' 請勿變更此程式碼。在上面的 Dispose(ByVal disposing As Boolean) 中輸入清除程式碼。
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' 由 Visual Basic 新增此程式碼以正確實作可處置的模式。
        Public Sub Dispose() Implements IDisposable.Dispose
            ' 請勿變更此程式碼。在以上的 Dispose 置入清除程式碼 (視為布林值處置)。
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class
End Namespace
