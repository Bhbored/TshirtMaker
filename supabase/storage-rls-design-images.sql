-- Storage RLS policies for the "Design images" bucket
-- Run this in the Supabase Dashboard → SQL Editor
--
-- Prerequisites:
-- 1. Create a bucket named exactly "Design images" in Storage (if it doesn't exist).
-- 2. The app must call Storage with the **Supabase Auth user JWT** so auth.uid() is set.
-- 3. Your upload path must start with the Supabase Auth user id: {auth.uid()}/final/... or {auth.uid()}/temp/...
--
-- INSERT/UPDATE/DELETE: only the owner (path starts with auth.uid()).
-- SELECT: any authenticated user can read (for feeds, shared designs, etc.).

-- Allow INSERT: upload to own folder only ({userId}/temp/... or {userId}/final/...)
CREATE POLICY "Users can upload to own folder in Design images"
ON storage.objects FOR INSERT
TO authenticated
WITH CHECK (
  bucket_id = 'Design images'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

-- Allow SELECT: any authenticated user can read any image in the bucket (signed URLs, list, etc.)
DROP POLICY IF EXISTS "Users can read own folder in Design images" ON storage.objects;
DROP POLICY IF EXISTS "All authenticated users can read Design images" ON storage.objects;
CREATE POLICY "All authenticated users can read Design images"
ON storage.objects FOR SELECT
TO authenticated
USING (bucket_id = 'Design images');

-- Allow UPDATE: overwrite/update own files
CREATE POLICY "Users can update own folder in Design images"
ON storage.objects FOR UPDATE
TO authenticated
USING (
  bucket_id = 'Design images'
  AND (storage.foldername(name))[1] = auth.uid()::text
)
WITH CHECK (
  bucket_id = 'Design images'
  AND (storage.foldername(name))[1] = auth.uid()::text
);

-- Allow DELETE: remove own files
CREATE POLICY "Users can delete own folder in Design images"
ON storage.objects FOR DELETE
TO authenticated
USING (
  bucket_id = 'Design images'
  AND (storage.foldername(name))[1] = auth.uid()::text
);
