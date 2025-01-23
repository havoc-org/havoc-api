using Havoc_API.DTOs.Assignment;
using Havoc_API.Models;

namespace Havoc_API.Services;

public interface IAssignmentService
{
    public Task<IEnumerable<AssignmentGET>> AddManyAssignmentsAsync(IEnumerable<AssignmentPOST> assignments, int taskId, int projectId);
    public Task<int> DeleteAssignmentAsync( int taskId, int userId, int projectId);
    public Task<bool> AddAssignmentAsync(AssignmentPOST assignment, int taskId);
}
