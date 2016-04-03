//------------------------------------------------------------------------------
//
// Copyright (c) 2015 Microsoft Corporation. All rights reserved.
//
// This code is licensed under the MIT License (MIT).
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
//------------------------------------------------------------------------------
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
//------------------------------------------------------------------------------

#include "pch.h"
#include "FBObject.h"

using namespace winsdkfb;
using namespace winsdkfb::Graph;
using namespace Platform;
using namespace Windows::Data::Json;
using namespace Windows::Foundation::Collections;
FBObject::FBObject(
    ) :
    _id(nullptr)
{
    ;
}

String^ FBObject::Id::get()
{
    return _id;
}
void FBObject::Id::set(String^ value)
{
    _id = value;
}

Object^ FBObject::FromJson(
    String^ JsonText 
    )
{
    FBObject^ result = ref new FBObject;
    int found = 0;
    JsonValue^ val = nullptr;

    if (JsonValue::TryParse(JsonText, &val))
    {
        if (val->ValueType == JsonValueType::Object)
        {
            JsonObject^ obj = val->GetObject();
            IIterator<IKeyValuePair<String^, IJsonValue^>^>^ it = nullptr;
            for (it = obj->First(); it->HasCurrent; it->MoveNext())
            {
                String^ key = it->Current->Key;
                if  (!String::CompareOrdinal(key, L"id"))
                {
                    found++;
                    result->Id = it->Current->Value->GetString();
                }
            }

            if (!found)
            {
                // No field names matched any known properties for this class.  
                // Even if it *is* an object of our type, it's not useful.
                result = nullptr;
            }
        }
    }
    return result;
}


