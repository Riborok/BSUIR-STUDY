#pragma once

#include <optional>
#include <functional>
#include <mutex>
#include <queue>

class JobQueue {
    std::queue<std::function<void()>> _jobs;
    std::mutex _mtx;
public:
    void enqueue(const std::function<void()>& job);
    std::optional<std::function<void()>> dequeue();
    bool empty();
};
