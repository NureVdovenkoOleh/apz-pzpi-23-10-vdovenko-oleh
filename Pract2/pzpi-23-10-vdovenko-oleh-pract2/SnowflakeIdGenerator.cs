using System;

public class SnowflakeIdGenerator
{
    private const long Twepoch = 1288834974657L; // Эпоха Twitter (4 ноября 2010 года)
    private const int WorkerIdBits = 10;
    private const int SequenceBits = 12;
    
    private const long MaxWorkerId = -1L ^ (-1L << WorkerIdBits);
    private const long SequenceMask = -1L ^ (-1L << SequenceBits);
    
    private const int WorkerIdShift = SequenceBits;
    private const int TimestampLeftShift = SequenceBits + WorkerIdBits;

    private readonly long _workerId;
    private long _sequence = 0L;
    private long _lastTimestamp = -1L;
    private readonly object _lock = new object();

    public SnowflakeIdGenerator(long workerId)
    {
        if (workerId > MaxWorkerId || workerId < 0)
        {
            throw new ArgumentException($"Worker ID должен быть в диапазоне от 0 до {MaxWorkerId}");
        }
        _workerId = workerId;
    }

    public long NextId()
    {
        lock (_lock)
        {
            long timestamp = GetCurrentTimestamp();

            if (timestamp < _lastTimestamp)
            {
                throw new Exception("Системное время переместилось назад. Генерация невозможна.");
            }

            if (_lastTimestamp == timestamp)
            {
                _sequence = (_sequence + 1) & SequenceMask;
                if (_sequence == 0)
                {
                    timestamp = WaitNextMillis(_lastTimestamp);
                }
            }
            else
            {
                _sequence = 0L;
            }

            _lastTimestamp = timestamp;

            // Формирование 64-битного ID с помощью побитовых сдвигов
            return ((timestamp - Twepoch) << TimestampLeftShift) |
                   (_workerId << WorkerIdShift) |
                   _sequence;
        }
    }

    private long GetCurrentTimestamp()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    private long WaitNextMillis(long lastTimestamp)
    {
        long timestamp = GetCurrentTimestamp();
        while (timestamp <= lastTimestamp)
        {
            timestamp = GetCurrentTimestamp();
        }
        return timestamp;
    }
}
