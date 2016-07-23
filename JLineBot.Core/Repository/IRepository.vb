Imports System.Linq.Expressions

Namespace Repository


    Public Interface IRepository(Of TEntity As Class)
        Inherits IDisposable

        '2016-02-13 新增後回傳物件
        'Sub Create(instance As TEntity)
        'Sub Update(instance As TEntity)

        Function Create(instance As TEntity) As TEntity
        Function Update(instance As TEntity) As TEntity

        Sub Delete(instance As TEntity)

        'TEntity Get(Expression<Func<TEntity, bool>> predicate);
        Function GetOne( _
         predicate As Expression(Of Func(Of TEntity, Boolean))) As TEntity

        Function GetAll() As IQueryable(Of TEntity)

        Sub SaveChanges()

        Function AsQuerable() As IQueryable(Of TEntity)

        'public virtual IQueryable<TEntity> AsQuerable() { return _unitOfWork.CreateSet<TEntity>().AsQueryable(); }
    End Interface
End Namespace
