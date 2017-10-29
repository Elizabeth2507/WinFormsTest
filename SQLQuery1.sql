select distinct Position, Avg(Salary) as AvarageSalary
	from dbo.EmployeeInfo
	group by Position;
