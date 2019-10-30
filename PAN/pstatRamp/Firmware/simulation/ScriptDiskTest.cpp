/*************************************************************************************************/
/*   Project : IO                                                                                */
/*   Authors : William Chung-How                                                                 */
/* Reviewers : Chris Dawber                                                                      */
/*************************************************************************************************/
#pragma once
#include "stdafx.h"
#include "gtest/gtest.h"
#include "ScriptDisk.h"
#include "ScriptDiskTest.hpp"



//=================================================================================================
static bool TestScriptDiskStore(const int filesize, char * filename) 
{
    int freeSpace = ScriptDiskGetFreeSpace(filename);
    // Check for duplicate name
    if (ScriptDiskIsDuplicate(filename))
    {
        return false;
    }
    if (filesize > freeSpace) 
    {
        return false;
    } 
    else 
    {
        ScriptDiskWriteFileTable(filename);     
        for (int i =0; i<filesize-1; i++)
        {
            ScriptDiskStoreData('-');      // Dummy data
        }        
        ScriptDiskSetFileLength();   
        return true;
    }    
}

//=================================================================================================
bool ScriptDiskTest::CompareFile(char * file1, char * file2)
{
    char filename1[64];
    char filename2[64];
    sprintf(filename1, "./Scripts/%s", file1);
    sprintf(filename2, "./Scripts/%s", file2);
    FILE * fp1 = fopen(filename1, "rb");
    FILE * fp2 = fopen(filename2, "rb");
    bool bRetVal = true;
    const int N = 10000;
    char buf1[N];
    char buf2[N];
    if (fp1 && fp2)
    {
        do {
            size_t r1 = fread(buf1, 1, N, fp1);
            size_t r2 = fread(buf2, 1, N, fp2);

            if (r1 != r2 ||
                memcmp(buf1, buf2, r1)) {
                bRetVal = false;  // Files are not equal
                break;
            }
        } while (!feof(fp1) && !feof(fp2));
        fclose(fp1);
        fclose(fp2);
    }
    else
    {
        bRetVal = false;
    }
    return bRetVal;
}

//=================================================================================================
void ScriptDiskTest::TestScriptDiskLoadFile(char * filename) {
    FILE * fp;
    int len;
    char readBuf[4096];
    char fileDisk[64];
    sprintf(fileDisk, "./Scripts/%s", filename);

    fp = fopen(fileDisk, "rb");
    if (fp) {
        fseek(fp, 0, SEEK_END);
        len = ftell(fp);
        ASSERT_LT(len, (int)sizeof(readBuf));
        fseek(fp, 0, SEEK_SET);
        fread(readBuf, len, 1, fp);
        fclose(fp);

        ScriptDiskWriteFileTable(filename);     
        for (int i =0; i<len; i++)
        {
            ScriptDiskStoreData(readBuf[i]);
        }        
        ScriptDiskSetFileLength(); 
    } else {
        printf ("File %s not found\r\n", filename);
    }  
    fclose(fp);
}


//=================================================================================================
// Check memory full - 28k on dev board, 56k on actual hardware
TEST_F(ScriptDiskTest, MemoryFull) {
    EXPECT_TRUE(TestScriptDiskStore(10240, "test1"));
    EXPECT_TRUE(TestScriptDiskStore(1024 * 40, "test2"));
    EXPECT_FALSE(TestScriptDiskStore(10240, "test3"));
}

//=================================================================================================
TEST_F(ScriptDiskTest, ScriptId) {
    char nameBuf[MAX_NUM_SCRIPT][16];
    for (int i =0; i<MAX_NUM_SCRIPT; i++) {
        sprintf(&nameBuf[i][0], "test%d", i);
        EXPECT_TRUE(TestScriptDiskStore(50, &nameBuf[i][0]));
    }
    
    // Test script id
    for (int i =0; i<MAX_NUM_SCRIPT; i++) {
        EXPECT_EQ(i, ScriptDiskGetId(&nameBuf[i][0]).id);
    }
}

//=================================================================================================
TEST_F(ScriptDiskTest, GetOffset) {
    EXPECT_TRUE(TestScriptDiskStore(1000, "test1"));
    EXPECT_TRUE(TestScriptDiskStore(1024, "test2"));
    EXPECT_TRUE(TestScriptDiskStore(3124, "test3"));
    EXPECT_EQ(0, ScriptDiskGetOffset(0));
    EXPECT_EQ(1000, ScriptDiskGetOffset(1));
    EXPECT_EQ(1000+1024, ScriptDiskGetOffset(2));
}

//=================================================================================================
TEST_F(ScriptDiskTest, GetLine) {
    char line1Buf[64];
    char line2Buf[64];
    char testBuf[64];
    memset(line1Buf, 0, sizeof(line1Buf));
    memset(line2Buf, 0, sizeof(line1Buf));
    memset(testBuf, 0, sizeof(line1Buf));
    char * pTestLine;

    sprintf(line1Buf, "Line1: Testing getting line from script memory\n");
    sprintf(line2Buf, "Line2: Line2 from script memory\n");

    ScriptDiskWriteFileTable("test"); 
    for (size_t i =0; i<strlen(line1Buf); i++) {
        ScriptDiskStoreData(line1Buf[i]);
    }
    for (size_t i =0; i<strlen(line2Buf); i++) {
        ScriptDiskStoreData(line2Buf[i]);
    }
    
    ScriptDiskSetFileLength();
    pTestLine = (char *)ScriptDiskGetLine(strlen(line1Buf));
    for (size_t i=0; i<strlen(line2Buf); i++) {
        testBuf[i] = pTestLine[i];
    }

    EXPECT_STREQ(line2Buf, testBuf);

}


//=================================================================================================
TEST_F(ScriptDiskTest, ExceededMaxScripts) {
    char buf[32];
    for (int i=0; i<MAX_NUM_SCRIPT; i++)
    {
        sprintf(buf, "test%d", i);
        EXPECT_TRUE(ScriptDiskHasEmptySlots());
        TestScriptDiskStore(50, buf);
    }
    EXPECT_FALSE(ScriptDiskHasEmptySlots());
}

//=================================================================================================
TEST_F(ScriptDiskTest, Duplicate) {
    EXPECT_TRUE(TestScriptDiskStore(1000, "test"));
    EXPECT_FALSE(TestScriptDiskStore(1024, "test"));
}