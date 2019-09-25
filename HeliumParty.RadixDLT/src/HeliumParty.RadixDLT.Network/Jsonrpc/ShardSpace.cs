using System;
using System.Collections.Generic;

namespace HeliumParty.RadixDLT.Jsonrpc
{
    public class ShardSpace
    {
        // TODO this class could probably use some comments...

        public const int ShardChunks = 1 << 20;
        public const long ShardChunkRange = -(long.MinValue / ShardChunks) * 2;
        public const long ShardChunkHalfRange = -(long.MinValue / ShardChunks);

        public static readonly ShardRange ShardRangeFull = new ShardRange(-ShardChunkHalfRange, ShardChunkHalfRange - 1);

        public long Anchor { get; }
        public ShardRange Range { get; }

        public ShardSpace(long anchor, long range)
        {
            if (range < 0)
                throw new ArgumentOutOfRangeException($"{nameof(range)} must not be negative");

            if (range > ShardChunkRange)
                throw new ArgumentOutOfRangeException($"{nameof(range)} must not be greater than {nameof(ShardChunkRange)}({ShardChunkRange})");

            Anchor = anchor;

            var chunkOffset = anchor % ShardChunkHalfRange;
            var low = chunkOffset - (range / 2);
            var high = low + range;

            var lowRemainder = 0L;
            if (low < -ShardChunkHalfRange)
            {
                lowRemainder = -ShardChunkHalfRange - low;
                low += lowRemainder;
            }

            long highRemainder = 0L;
            if (high > ShardChunkHalfRange)
            {
                highRemainder = high - ShardChunkHalfRange;
                high -= highRemainder;
            }

            low -= highRemainder;
            high += lowRemainder;
            Range = new ShardRange(low, high);
        }

        public ShardSpace(long anchor, ShardRange range)
        {
            if (range.GetSpan() < 0)
                throw new ArgumentException($"{nameof(range)} must not be negative");

            if (range.GetSpan() > ShardChunkRange)
                throw new ArgumentOutOfRangeException($"{nameof(range)} must not be greater than {nameof(ShardChunkRange)}({ShardChunkRange})");

            Anchor = anchor;
            var chunkOffset = anchor % ShardChunkHalfRange;
            Range = new ShardRange(range.Low, range.High);
        }

        public static int ToChunk(long shard)
        {
            shard &= ~1;
            return (int)(((shard / 2) + Math.Abs(long.MinValue / 2)) / ShardSpace.ShardChunkHalfRange);
        }

        public static long FromChunk(int chunk, long anchor)
        {
            if (anchor < -ShardSpace.ShardChunkHalfRange || anchor > ShardSpace.ShardChunkHalfRange)
                throw new ArgumentOutOfRangeException($"{nameof(anchor)} is invalid");

            return (long.MinValue + ShardSpace.ShardChunkRange * chunk) + anchor;
        }

        public bool Intersects(long shard)
        {
            long remainder = shard % ShardChunkHalfRange;
            return this.Range.Intersects(remainder);
        }

        public bool Intersects(IEnumerable<long> shards)
        {
            foreach (var shard in shards)
            {
                if (this.Range.Intersects(shard % ShardChunkHalfRange))
                    return true;
            }

            return false;
        }

        public bool Intersects(ShardRange shardRange) => this.Range.Intersects(shardRange);

        public bool Intersects(ShardSpace shardSpace) => this.Range.Intersects(shardSpace.Range);

        public HashSet<long> Intersection(ICollection<long> shards)
        {
            if (shards == null)
                throw new System.ArgumentNullException(nameof(shards));

            HashSet<long> intersections = new HashSet<long>();
            foreach (var shard in shards)
            {
                if (!intersections.Contains(shard) && this.Intersects(shard))
                    intersections.Add(shard);
            }

            return intersections;
        }
        
        public override string ToString() => $"Anchor: {Anchor} Range: {Range.Low} -> {Range.High}";
    }
}
