
set scn=5

set /p=Running Scenario %scn%. Press ENTER to continue...
set drive=C:
set workingdir=C:\BRM\LANDIS_II\Code\GitCode\Extension-SpruceBudworm\deploy\examples\SBW_TEST

if not exist %workingdir%\Scn%scn%b mkdir %workingdir%\Scn%scn%b
%drive%
cd %workingdir%\Scn%scn%b
copy %workingdir%\Scenario_SBW_Test_Scn%scn%b.txt
call landis Scenario_SBW_Test_Scn%scn%b.txt
%drive%
cd %workingdir%\Scn%scn%b
