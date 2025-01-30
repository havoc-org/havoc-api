using Havoc_API.Data;
using Havoc_API.DTOs.Assignment;
using Havoc_API.DTOs.User;
using Havoc_API.Exceptions;
using Havoc_API.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Havoc_API.Services;
public class AssignmentService : IAssignmentService
{
    private readonly IHavocContext _havocContext;

    public AssignmentService(HavocContext havocContext)
    {
        _havocContext = havocContext;

    }

    public async Task<bool> AddAssignmentAsync(AssignmentPOST assignment, int taskId)
    {

        var user = await _havocContext.Users.FirstOrDefaultAsync(us => us.UserId == assignment.UserId);
        if (user == null)
        {
            return false;
            throw new Exception("User not found");
        }
        var existingAssignment = await _havocContext.Assignments.FindAsync(taskId, user.UserId);
        if (existingAssignment != null)
        {
            throw new Exception("This assignment already exists userID: " + existingAssignment.UserId + " projectID: " + existingAssignment.TaskId);
        }

        var task = await _havocContext.Tasks.FindAsync(taskId);
        if (task == null) { throw new Exception("Task not found");};

        var result = await _havocContext.Assignments.AddAsync(new Assignment(
            assignment.Description,
            task,
            user
        ));
         await _havocContext.SaveChangesAsync(); 
        return true;
    }

    public async Task<IEnumerable<AssignmentGET>> AddManyAssignmentsAsync(
    IEnumerable<AssignmentPOST> assignments,
    int taskId,
    int projectId)
    {
        try
        {

            if (await _havocContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId && t.ProjectId == projectId) is null)
                throw new NotFoundException("Task or Project doesn't exist");

            var newAssignments = new List<Assignment>();

            foreach (var assignment in assignments)
            {
                var existingAssignment = await _havocContext.Assignments
                    .FirstOrDefaultAsync(a => a.UserId == assignment.UserId && a.TaskId == taskId);

                if (existingAssignment == null)
                {
                    var task = await _havocContext.Tasks.FirstOrDefaultAsync(t => t.TaskId == taskId)
                               ?? throw new NotFoundException("Task doesn't exist");

                    var user = await _havocContext.Users.FirstOrDefaultAsync(u => u.UserId == assignment.UserId)
                               ?? throw new NotFoundException($"User with ID {assignment.UserId} doesn't exist");

                    var newAssignment = new Assignment(assignment.Description, task, user);
                    newAssignments.Add(newAssignment);
                }
            }

            if (newAssignments.Any())
            {
                await _havocContext.Assignments.AddRangeAsync(newAssignments);
                await _havocContext.SaveChangesAsync();
            }

            return newAssignments.Select(a =>
                new AssignmentGET(
                    new UserGET(
                        a.UserId,
                        a.User.FirstName,
                        a.User.LastName,
                        a.User.Email
                    ),
                    a.Description
                )).ToList();
        }
        catch (SqlException e)
        {
            throw new DataAccessException(e.Message);
        }
        catch (DbUpdateException e)
        {
            throw new DataAccessException(e.Message);
        }
    }

    public async Task<int> DeleteAssignmentAsync(int taskId, int userId, int projectId)
    {
        try
        {

            var assignment = await _havocContext.Assignments
                .Include(a => a.Task)
                .FirstOrDefaultAsync(a => a.UserId == userId && a.TaskId == taskId && a.Task.ProjectId == projectId);

            if (assignment == null)
            {
                Console.WriteLine($"[Error] Assignment not found. UserId={userId}, TaskId={taskId}, ProjectId={projectId}");
                throw new NotFoundException($"Assignment for UserId={userId}, TaskId={taskId}, ProjectId={projectId} doesn't exist.");
            }

            _havocContext.Assignments.Remove(assignment);
            var result = await _havocContext.SaveChangesAsync();

            Console.WriteLine($"[Info] Assignment deleted successfully. UserId={userId}, TaskId={taskId}, ProjectId={projectId}");
            return result;
        }
        catch (SqlException e)
        {
            Console.WriteLine($"[Error] Database error occurred: {e.Message}");
            throw new DataAccessException($"Database error: {e.Message}");
        }
        catch (DbUpdateException e)
        {
            Console.WriteLine($"[Error] Database update error occurred: {e.Message}");
            throw new DataAccessException($"Database update error: {e.Message}");
        }
    }
    public async Task<int> DeleteManyAssignmentsAsync(IEnumerable<AssignmentDELETE> assignments, int taskId, int projectId)
    {
        try
        {

            foreach (var assignment in assignments)
            {
                var existingAssignment = await _havocContext.Assignments
                    .FirstOrDefaultAsync(a => a.UserId == assignment.UserId && a.TaskId == taskId);

                if (existingAssignment == null)
                {
                    Console.WriteLine($"[Error] Assignment not found. UserId={assignment.UserId}, TaskId={taskId}, ProjectId={projectId}");
                    throw new NotFoundException($"Assignment for UserId={assignment.UserId}, TaskId={taskId}, ProjectId={projectId} doesn't exist.");
                }

                _havocContext.Assignments.Remove(existingAssignment);
            }

            var result = await _havocContext.SaveChangesAsync();

            Console.WriteLine($"[Info] Assignment deleted successfully. UsersIds={assignments.Select(o=>o.UserId)}, TaskId={taskId}, ProjectId={projectId}");
            return result;
        }
        catch (SqlException e)
        {
            Console.WriteLine($"[Error] Database error occurred: {e.Message}");
            throw new DataAccessException($"Database error: {e.Message}");
        }
        catch (DbUpdateException e)
        {
            Console.WriteLine($"[Error] Database update error occurred: {e.Message}");
            throw new DataAccessException($"Database update error: {e.Message}");
        }
    }
}