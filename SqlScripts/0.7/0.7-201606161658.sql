declare @a float;
declare @b int;

set @a = null
set @b = 0

select
case when @a = @b then 'true' else 'false' end