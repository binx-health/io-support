/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : William Chung-How                                                                 */
/* Reviewers : Chris Dawber                                                                      */
/*************************************************************************************************/
#include "ScriptDisk.h"
// Class definition
//=================================================================================================
class ScriptDiskTest : public testing::Test {
    public:
        virtual void SetUp() { ScriptDiskInit(); }
        virtual void TeadDown() { ScriptDiskInit(); }
        void TestScriptDiskLoadFile(char * filename);
        bool CompareFile(char * file1, char * file2);
};