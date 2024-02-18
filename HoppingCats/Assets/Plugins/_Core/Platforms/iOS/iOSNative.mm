// Helper method to create C string copy
char* MakeStringCopy (const char* string)
{
	if (string == NULL)
		return NULL;
	
	char* res = (char*)malloc(strlen(string) + 1);
	strcpy(res, string);
	return res;
}

extern "C" {

	const char* GetVersionCode_iOS()
	{
		return MakeStringCopy([[[NSBundle mainBundle] objectForInfoDictionaryKey:@"CFBundleVersion"] UTF8String]);
	}
}