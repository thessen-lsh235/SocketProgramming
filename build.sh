#!/bin/bash

echo "í ½í´§ ë¹Œë“œ ë””ë ‰í† ë¦¬ ìƒì„± ì¤‘..."
mkdir -p build
cd build

echo "í ½í» ï¸ CMake êµ¬ì„± ì¤‘..."
cmake ..

echo "í ½í³¦ ë¹Œë“œ ì‹œì‘..."
make -j$(nproc)

echo "âœ… ë¹Œë“œ ì™„ë£Œ!"

