/*
We are making an action game (2D/3D is irrelevant).
In game we need to make coins appear when the player destroys enemies or objects.
Please implement an object pool so that we don't have to allocate those coins all the time.
In this question we are only interested in the pool logic (api and implementation) so there is no need to worry about details such as rendering and effects. 
The implementation is up to you but please try to fulfill at least those conditions:

 - 10000 coins are allocated at init time
 - no allocation happens after that during the game
 - coins disapears when the player takes them
 - coins disappears after 300 frame

class Coin
{
}

class CoinObjectPool
{
}

Please answer in C++ (or C# if you are not familiar with C++).

*/


#include <iostream>
#include <vector>
#include <queue>

using namespace std;

class Coin {
private:
    bool inUse;

public:
    // Constructor initializes the coin as inactive
    Coin() : inUse(false) {}

    void activate() {
        inUse = true;
    }

    void deactivate() {
        inUse = false;
    }

    bool isInUse() const {
        return inUse;
    }
};


class CoinObjectPool {
private:
    // Storage for all coin objects
    vector<Coin> coins;

    // Indices of available (inactive) coins
    queue<int> availableIndices;

    // Pairs of (index, activation frame)
    vector<pair<int, int>> activeCoins;

    // Tracks the current frame number
    int currentFrame;

    // Initializes the queue with all coin indices
    void initializeAvailableIndices() {
        for (int i = 0; i < 10000; ++i) {
            availableIndices.push(i);
        }
    }

public:
    // Constructor initializes 10,000 coins and sets the current frame to 0
    CoinObjectPool() : coins(10000), currentFrame(0) {
        initializeAvailableIndices();
    }

    // Retrieves an available coin from the pool
    Coin* getCoin() {
        if (availableIndices.empty()) {
            return nullptr; // No available coins
        }
        int index = availableIndices.front();
        availableIndices.pop();
        coins[index].activate();
        activeCoins.push_back({ index, currentFrame });
        return &coins[index];
    }

    // Returns a coin to the pool, making it available again
    void releaseCoin(Coin* coin) {
        for (auto it = activeCoins.begin(); it != activeCoins.end(); ++it) {
            if (&coins[it->first] == coin) {
                int index = it->first;
                coins[index].deactivate();
                activeCoins.erase(it);
                availableIndices.push(index);
                return;
            }
        }
    }

    // Updates the state of all active coins; deactivates coins active for 300 frames
    void update() {
        currentFrame++;

        for (auto it = activeCoins.begin(); it != activeCoins.end(); ) {
            if (currentFrame - it->second >= 300) {
                int index = it->first;
                coins[index].deactivate();
                availableIndices.push(index);
                it = activeCoins.erase(it);
            }
            else {
                ++it;
            }
        }
    }

    // Returns a list of pointers to all active coins
    vector<Coin*> getActiveCoins() {
        vector<Coin*> activeCoinPointers;
        for (const auto& entry : activeCoins) {
            activeCoinPointers.push_back(&coins[entry.first]);
        }
        return activeCoinPointers;
    }
};

