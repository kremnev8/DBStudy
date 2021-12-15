using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer.Utilities;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using VendingMachineBackend.Models;

namespace VendingMachineBackend
{
    public class ApplicationUserStore :
        IUserPasswordStore<Employee, string>,
        IQueryableUserStore<Employee, string>,
        IUserEmailStore<Employee, string>,
        IUserStore<Employee>
    {
        private bool _disposed;

        /// <summary>
        ///     Constructor which takes a db context and wires up the stores with default instances using the context
        /// </summary>
        /// <param name="context"></param>
        public ApplicationUserStore(VendingBusinessContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            AutoSaveChanges = true;
        }

        /// <summary>Context for the store</summary>
        public VendingBusinessContext Context { get; private set; }

        /// <summary>
        ///     If true will call dispose on the DbContext during Dispose
        /// </summary>
        public bool DisposeContext { get; set; }

        /// <summary>
        ///     If true will call SaveChanges after Create/Update/Delete
        /// </summary>
        public bool AutoSaveChanges { get; set; }

        /// <summary>Returns an IQueryable of users</summary>
        public IQueryable<Employee> Users => Context.employee;
        

        /// <summary>Returns whether the user email is confirmed</summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<bool> GetEmailConfirmedAsync(Employee user)
        {
            ThrowIfDisposed();
            return Task.FromResult(true);
        }

        /// <summary>Set IsConfirmed on the user</summary>
        /// <param name="user"></param>
        /// <param name="confirmed"></param>
        /// <returns></returns>
        public virtual Task SetEmailConfirmedAsync(Employee user, bool confirmed)
        {
            ThrowIfDisposed();
            return Task.FromResult(0);
        }

        /// <summary>Set the user email</summary>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual Task SetEmailAsync(Employee user, string email)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            user.Email = email;
            return Task.FromResult(0);
        }

        /// <summary>Get the user's email</summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<string> GetEmailAsync(Employee user)
        {
            ThrowIfDisposed();
            return (object) user != null ? Task.FromResult(user.Email) : throw new ArgumentNullException(nameof(user));
        }

        /// <summary>Find a user by email</summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public virtual Task<Employee> FindByEmailAsync(string email)
        {
            ThrowIfDisposed();
            return GetUserAggregateAsync(u => u.Email.ToUpper() == email.ToUpper());
        }

        /// <summary>Find a user by id</summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual Task<Employee> FindByIdAsync(string userId)
        {
            ThrowIfDisposed();
            return GetUserAggregateAsync(u => u.Id.Equals(userId));
        }
        

        /// <summary>Find a user by name</summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public virtual Task<Employee> FindByNameAsync(string userName)
        {
            ThrowIfDisposed();
            return GetUserAggregateAsync(u => u.FullName.ToUpper() == userName.ToUpper());
        }

        /// <summary>Insert an entity</summary>
        /// <param name="user"></param>
        public virtual async Task CreateAsync(Employee user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            Context.employee.Add(user);
            await SaveChanges().WithCurrentCulture();
        }

        /// <summary>Mark an entity for deletion</summary>
        /// <param name="user"></param>
        public virtual async Task DeleteAsync(Employee user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            Context.employee.Remove(user);
            await SaveChanges().WithCurrentCulture();
        }

        /// <summary>Update an entity</summary>
        /// <param name="user"></param>
        public virtual async Task UpdateAsync(Employee user)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            Context.Entry(user).State = EntityState.Modified;
            
            await SaveChanges().WithCurrentCulture();
        }

        /// <summary>Dispose the store</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Set the password hash for a user</summary>
        /// <param name="user"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public virtual Task SetPasswordHashAsync(Employee user, string passwordHash)
        {
            ThrowIfDisposed();
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            user.Password = passwordHash;
            return Task.FromResult(0);
        }

        /// <summary>Get the password hash for a user</summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<string> GetPasswordHashAsync(Employee user)
        {
            ThrowIfDisposed();
            return (object) user != null ? Task.FromResult(user.Password) : throw new ArgumentNullException(nameof(user));
        }

        /// <summary>Returns true if the user has a password set</summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public virtual Task<bool> HasPasswordAsync(Employee user) => Task.FromResult(user.Password != null);

        private async Task SaveChanges()
        {
            if (!AutoSaveChanges)
                return;
            int num = await Context.SaveChangesAsync().WithCurrentCulture();
        }

        /// <summary>
        /// Used to attach child entities to the User aggregate, i.e. Roles, Logins, and Claims
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        protected virtual async Task<Employee> GetUserAggregateAsync(
            Expression<Func<Employee, bool>> filter)
        {
            Employee user;
            int id;
            if (FindByIdFilterParser.TryMatchAndGetId(filter, out id))
                
                user = await Context.employee.FindAsync(id).WithCurrentCulture();
            else
                user = await Users.FirstOrDefaultAsync(filter).WithCurrentCulture();

            return user;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        ///     If disposing, calls dispose on the Context.  Always nulls out the Context
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (DisposeContext & disposing)
                Context?.Dispose();
            _disposed = true;
            Context = null;
        }

        private static class FindByIdFilterParser
        {
            private static readonly Expression<Func<Employee, bool>> Predicate = u => u.Id.Equals(default);

            private static readonly MethodInfo EqualsMethodInfo =
                ((MethodCallExpression) Predicate.Body).Method;

            private static readonly MemberInfo UserIdMemberInfo =
                ((MemberExpression) ((MethodCallExpression) Predicate
                    .Body).Object).Member;

            internal static bool TryMatchAndGetId(Expression<Func<Employee, bool>> filter, out int id)
            {
                id = default;
                if (filter.Body.NodeType != ExpressionType.Call)
                    return false;
                MethodCallExpression body = (MethodCallExpression) filter.Body;
                if (body.Method != EqualsMethodInfo ||
                    body.Object == null || (body.Object.NodeType != ExpressionType.MemberAccess || ((MemberExpression) body.Object).Member !=
                        UserIdMemberInfo) || body.Arguments.Count != 1)
                    return false;
                MemberExpression operand;
                if (body.Arguments[0].NodeType == ExpressionType.Convert)
                {
                    UnaryExpression unaryExpression = (UnaryExpression) body.Arguments[0];
                    if (unaryExpression.Operand.NodeType != ExpressionType.MemberAccess)
                        return false;
                    operand = (MemberExpression) unaryExpression.Operand;
                }
                else
                {
                    if (body.Arguments[0].NodeType != ExpressionType.MemberAccess)
                        return false;
                    operand = (MemberExpression) body.Arguments[0];
                }

                if (operand.Member.MemberType != MemberTypes.Field || operand.Expression.NodeType != ExpressionType.Constant)
                    return false;
                FieldInfo member = (FieldInfo) operand.Member;
                object obj = ((ConstantExpression) operand.Expression).Value;
                id = (int) member.GetValue(obj);
                return true;
            }
        }
    }


    public class ApplicationUserManager : UserManager<Employee>
    {
        public ApplicationUserManager(IUserStore<Employee> store): base(store)
        {
            ((UserValidator<Employee, string>) UserValidator).AllowOnlyAlphanumericUserNames = false;
            PasswordHasher = new FakePasswordHasher();
        }
        
        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var store = new ApplicationUserStore(context.Get<VendingBusinessContext>());
            var manager = new ApplicationUserManager(store);
            return manager;
        }
        
        public override async Task<Employee> FindAsync(string userName, string password)
        {
            Employee user = await FindByEmailAsync(userName).WithCurrentCulture();
            return (object) user == null ? default : await CheckPasswordAsync(user, password).WithCurrentCulture() ? user : default;
        }
    }
    
    
    
    public class ApplicationSignInManager : SignInManager<Employee, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
        }
 
        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
        
        public override async Task<SignInStatus> PasswordSignInAsync(
            string userName,
            string password,
            bool isPersistent,
            bool shouldLockout)
        {
            if (UserManager == null)
                return SignInStatus.Failure;
            Employee user = await UserManager.FindByEmailAsync(userName).WithCurrentCulture();
            if (user == null)
                return SignInStatus.Failure;

            if (await UserManager.CheckPasswordAsync(user, password).WithCurrentCulture())
            {
                await SignInAsync(user, isPersistent, false).WithCurrentCulture();
                return SignInStatus.Success;
            }
            return SignInStatus.Failure;
        }
    }

    public class FakePasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return password;
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword.Equals(providedPassword))
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }
    }
    
    
}