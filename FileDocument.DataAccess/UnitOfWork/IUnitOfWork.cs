using FileDocument.DataAccess.IRepository;

namespace FileDocument.DataAccess.UnitOfWork
{
    public interface IUnitOfWork
    {
        IAddressRepository Address { get; }
        IUserRepository User { get; }
        IAuthRepository Authenticate { get; }
        IGroupRepository Group { get; }
        IGroupMemberRepository GroupUser { get; }
        IPermissionRepoitory Permission { get; }
        ISystemConfigureRepository SystemConfigure { get; }
        IGroupDocTypePermissionRepository GroupDocTypePermission { get; }
        IDocumentTypeRepository DocumentType { get; }
        IAircraftRepository Aircraft { get; }
        IAirportRepository Airport { get; }
        IFlightRepository Flight { get; }
        Task DisposeAsync();
        Task<int> SaveChangesAsync();
    }
}
