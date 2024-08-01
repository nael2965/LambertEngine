#pragma once

#include "Test.h"
#include "../Engine/Components/Entity.h"
#include "../Engine/Components/Transform.h"
//#include "../Engine/Utilities/Utilities.h"

using namespace lambert;
class engine_test : public test
{
public:
    bool initialize() override{ return true; }
    void run() override{}
    void shutdown() override{}
};